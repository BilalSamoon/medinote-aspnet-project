using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;
using MediNote.Web.ViewModels;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Provides business logic for doctor appointment actions.
    /// </summary>
    public class DoctorAppointmentService
    {
        private readonly MediNoteDbContext _context;
        private readonly PriorityCalculationService _priorityCalculationService;
        private readonly NotificationService? _notificationService;

        public DoctorAppointmentService(MediNoteDbContext context, PriorityCalculationService priorityCalculationService)
        {
            _context = context;
            _priorityCalculationService = priorityCalculationService;
        }

        public DoctorAppointmentService(MediNoteDbContext context, PriorityCalculationService priorityCalculationService, NotificationService notificationService)
            : this(context, priorityCalculationService)
        {
            _notificationService = notificationService;
        }

        public PendingAppointmentsViewModel GetPendingAppointmentsViewModel(string? doctorName = null, bool isAdmin = false)
        {
            var query = _context.Appointments.Where(a => a.Status == "Pending");

            if (!isAdmin && !string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            var pendingDb = query
                .OrderBy(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();

            var vm = new PendingAppointmentsViewModel();

            foreach (var appt in pendingDb)
            {
                vm.PendingAppointments.Add(new PendingAppointmentItemViewModel
                {
                    AppointmentId = appt.AppointmentId,
                    PatientName = appt.PatientName,
                    DoctorName = appt.DoctorName,
                    RequestedDate = appt.RequestedDate,
                    RequestedTime = appt.RequestedTime,
                    Symptoms = appt.Symptoms,
                    Priority = _priorityCalculationService.GetPriority(appt.Symptoms),
                    Status = appt.Status
                });
            }

            return vm;
        }

        public List<Appointment> GetAppointmentsForManagement(string? doctorName = null, bool isAdmin = false)
        {
            var query = _context.Appointments.AsQueryable();

            if (!isAdmin && !string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            return query
                .OrderByDescending(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();
        }

        public string ApproveAppointment(int appointmentId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment != null)
            {
                appointment.Status = "Approved";
                appointment.LastUpdatedAtUtc = DateTime.UtcNow;
                _context.SaveChanges();
                _notificationService?.QueueStatusChange(appointment, appointment.Status, appointment.ContactRecipient, appointment.NotificationChannel);
                return $"Appointment #{appointmentId} was approved successfully.";
            }
            return $"Appointment #{appointmentId} not found.";
        }

        public string CancelAppointment(int appointmentId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment != null)
            {
                if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
                {
                    return $"Appointment #{appointmentId} cannot be cancelled in its current state.";
                }

                appointment.Status = "Cancelled";
                appointment.LastUpdatedAtUtc = DateTime.UtcNow;
                _context.SaveChanges();
                _notificationService?.QueueStatusChange(appointment, appointment.Status, appointment.ContactRecipient, appointment.NotificationChannel);
                return $"Appointment #{appointmentId} was cancelled successfully.";
            }
            return $"Appointment #{appointmentId} not found.";
        }

        public string RejectAppointment(int appointmentId)
        {
            return CancelAppointment(appointmentId);
        }

        public RescheduleAppointmentViewModel GetRescheduleViewModel(int appointmentId)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appt != null)
            {
                return new RescheduleAppointmentViewModel
                {
                    AppointmentId = appt.AppointmentId,
                    PatientName = appt.PatientName,
                    CurrentDate = appt.RequestedDate,
                    CurrentTime = appt.RequestedTime
                };
            }

            return new RescheduleAppointmentViewModel
            {
                AppointmentId = appointmentId,
                PatientName = "Unknown Patient",
                CurrentDate = DateTime.MinValue,
                CurrentTime = "Unavailable"
            };
        }

        public bool IsAppointmentEligibleForReschedule(int appointmentId)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            return appt != null && appt.Status != "Cancelled";
        }

        public bool HasRescheduleConflict(DateTime newDate, TimeSpan newTime)
        {
            return HasRescheduleConflict(newDate, newTime, null, null);
        }

        public bool HasRescheduleConflict(DateTime newDate, TimeSpan newTime, string? doctorName, int? ignoreAppointmentId)
        {
            var formattedTime = newTime.ToString(@"hh\:mm");
            var query = _context.Appointments.Where(a =>
                a.RequestedDate.Date == newDate.Date &&
                a.RequestedTime == formattedTime &&
                a.Status != "Cancelled");

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            if (ignoreAppointmentId.HasValue)
            {
                query = query.Where(a => a.AppointmentId != ignoreAppointmentId.Value);
            }

            return query.Any();
        }

        public bool TryValidateRescheduleSlot(string doctorName, DateTime newDate, TimeSpan newTime, int? ignoreAppointmentId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var requestedDateTime = newDate.Date.Add(newTime);
            if (requestedDateTime <= DateTime.Now)
            {
                errorMessage = "Appointments must be scheduled in the future.";
                return false;
            }

            var hasCoverage = _context.Availabilities.Any(slot =>
                slot.DoctorName == doctorName &&
                slot.AvailableDate.Date == newDate.Date &&
                newTime >= slot.StartTime &&
                newTime < slot.EndTime);

            if (!hasCoverage)
            {
                errorMessage = "The selected time is outside the doctor's published working hours for that day.";
                return false;
            }

            if (HasRescheduleConflict(newDate, newTime, doctorName, ignoreAppointmentId))
            {
                errorMessage = "The selected new time slot is already taken.";
                return false;
            }

            return true;
        }

        public string ConfirmReschedule(int appointmentId, DateTime newDate, TimeSpan newTime)
        {
            var formattedTime = newTime.ToString(@"hh\:mm");
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment != null)
            {
                appointment.RequestedDate = newDate.Date;
                appointment.RequestedTime = formattedTime;
                appointment.LastUpdatedAtUtc = DateTime.UtcNow;
                _context.SaveChanges();
                _notificationService?.QueueStatusChange(appointment, "Rescheduled", appointment.ContactRecipient, appointment.NotificationChannel);
                return $"Appointment #{appointmentId} was rescheduled successfully.";
            }

            return $"Failed to reschedule Appointment #{appointmentId}.";
        }
    }
}

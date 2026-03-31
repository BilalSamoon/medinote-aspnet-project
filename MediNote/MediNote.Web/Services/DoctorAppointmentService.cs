using System;
using System.Linq;
using MediNote.Web.ViewModels;
using MediNote.Web.Data;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Provides business logic for doctor appointment actions.
    /// </summary>
    public class DoctorAppointmentService
    {
        private readonly MediNoteDbContext _context;

        public DoctorAppointmentService(MediNoteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns pending appointments directly from the database for display.
        /// </summary>
        /// <returns>A populated pending appointments view model.</returns>
        public PendingAppointmentsViewModel GetPendingAppointmentsViewModel(string? doctorName = null, bool isAdmin = false)
        {
            var query = _context.Appointments.Where(a => a.Status == "Pending");
            
            if (!isAdmin && !string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            var pendingDb = query.ToList();
            var vm = new PendingAppointmentsViewModel();

            foreach(var appt in pendingDb)
            {
                vm.PendingAppointments.Add(new PendingAppointmentItemViewModel
                {
                    AppointmentId = appt.AppointmentId,
                    PatientName = appt.PatientName,
                    DoctorName = appt.DoctorName,
                    RequestedDate = appt.RequestedDate,
                    Symptoms = appt.Symptoms
                });
            }

            return vm;
        }

        /// <summary>
        /// Approves a selected appointment in the database.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string ApproveAppointment(int appointmentId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment != null)
            {
                appointment.Status = "Approved";
                _context.SaveChanges();
                return $"Appointment #{appointmentId} was approved successfully.";
            }
            return $"Appointment #{appointmentId} not found.";
        }

        /// <summary>
        /// Rejects a selected appointment in the database.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string RejectAppointment(int appointmentId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment != null)
            {
                appointment.Status = "Rejected";
                _context.SaveChanges();
                return $"Appointment #{appointmentId} was rejected successfully.";
            }
            return $"Appointment #{appointmentId} not found.";
        }

        /// <summary>
        /// Retrieves appointment details for the reschedule page.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A populated reschedule appointment view model.</returns>
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

        /// <summary>
        /// Checks whether an appointment is eligible for rescheduling.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>True if the appointment is eligible; otherwise false.</returns>
        public bool IsAppointmentEligibleForReschedule(int appointmentId)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            return appt != null && appt.Status != "Cancelled" && appt.Status != "Rejected";
        }

        /// <summary>
        /// Checks whether the selected new appointment slot conflicts with existing booked times in the database.
        /// </summary>
        /// <param name="newDate">The new selected date.</param>
        /// <param name="newTime">The new selected time.</param>
        /// <returns>True if there is a conflict; otherwise false.</returns>
        public bool HasRescheduleConflict(DateTime newDate, TimeSpan newTime)
        {
            var formattedTime = newTime.ToString(@"hh\:mm");
            
            return _context.Appointments.Any(a => 
                a.RequestedDate.Date == newDate.Date && 
                a.RequestedTime == formattedTime && 
                a.Status != "Cancelled" && 
                a.Status != "Rejected");
        }

        /// <summary>
        /// Saves a completed reschedule sequence to the database.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <param name="newDate">The new selected date.</param>
        /// <param name="newTime">The new selected time.</param>
        /// <returns>A success message.</returns>
        public string ConfirmReschedule(int appointmentId, DateTime newDate, TimeSpan newTime)
        {
            var formattedTime = newTime.ToString(@"hh\:mm");
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            
            if (appointment != null)
            {
                appointment.RequestedDate = newDate;
                appointment.RequestedTime = formattedTime;
                _context.SaveChanges();
                return $"Appointment #{appointmentId} was rescheduled successfully.";
            }
            
            return $"Failed to reschedule Appointment #{appointmentId}.";
        }
    }
}
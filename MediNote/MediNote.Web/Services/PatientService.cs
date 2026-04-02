using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Contracts;
using MediNote.Web.Data;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Patient service for patient-specific operations.
    /// </summary>
    public class PatientService
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly MediNoteDbContext? _context;
        private readonly NotificationService? _notificationService;

        public PatientService(AppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public PatientService(AppointmentRepository appointmentRepository, MediNoteDbContext context, NotificationService notificationService)
            : this(appointmentRepository)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public IList<Appointment> GetPatientAppointments(string patientName)
        {
            return _appointmentRepository.GetAppointmentsByPatient(patientName);
        }

        public PatientDashboardDto GetPatientDashboard(string patientName)
        {
            var appointments = _appointmentRepository.GetAppointmentsByPatient(patientName)
                .OrderByDescending(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();

            var upcoming = appointments
                .Where(a => a.Status != "Cancelled" && a.RequestedDate.Date >= DateTime.Today)
                .OrderBy(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .FirstOrDefault();

            return new PatientDashboardDto
            {
                PatientName = patientName,
                TotalAppointments = appointments.Count,
                PendingAppointments = appointments.Count(a => a.Status == "Pending"),
                ApprovedAppointments = appointments.Count(a => a.Status == "Approved" || a.Status == "Completed"),
                CancelledAppointments = appointments.Count(a => a.Status == "Cancelled"),
                NextAppointment = upcoming == null ? null : MapSummary(upcoming),
                Appointments = appointments.Select(MapSummary).ToList()
            };
        }

        public AppointmentDetailDto? GetAppointmentDetail(int appointmentId, string patientName, bool isAdmin = false)
        {
            var appointment = _appointmentRepository.GetAppointmentById(appointmentId);
            if (appointment == null)
            {
                return null;
            }

            if (!isAdmin && appointment.PatientName != patientName)
            {
                return null;
            }

            var context = EnsureContext();
            return new AppointmentDetailDto
            {
                Appointment = MapSummary(appointment),
                DoctorNotes = context.DoctorNotes
                    .Where(n => n.AppointmentId == appointmentId)
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .Select(n => new DoctorNoteDto
                    {
                        DoctorNoteId = n.DoctorNoteId,
                        AppointmentId = n.AppointmentId,
                        DoctorName = n.DoctorName,
                        Note = n.Note,
                        FollowUpInstructions = n.FollowUpInstructions,
                        CreatedAtUtc = n.CreatedAtUtc
                    }).ToList(),
                Prescriptions = context.Prescriptions
                    .Where(p => p.AppointmentId == appointmentId)
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .Select(p => new PrescriptionDto
                    {
                        PrescriptionId = p.PrescriptionId,
                        AppointmentId = p.AppointmentId,
                        DoctorName = p.DoctorName,
                        MedicationName = p.MedicationName,
                        Dosage = p.Dosage,
                        Frequency = p.Frequency,
                        Duration = p.Duration,
                        Instructions = p.Instructions,
                        CreatedAtUtc = p.CreatedAtUtc
                    }).ToList(),
                Notifications = context.NotificationLogs
                    .Where(n => n.AppointmentId == appointmentId)
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .Select(n => new NotificationLogDto
                    {
                        NotificationLogId = n.NotificationLogId,
                        AppointmentId = n.AppointmentId,
                        Channel = n.Channel,
                        Type = n.Type,
                        Recipient = n.Recipient,
                        Message = n.Message,
                        Status = n.Status,
                        CreatedAtUtc = n.CreatedAtUtc
                    }).ToList()
            };
        }

        public List<DoctorSlotDto> GetDoctorSlots(string? doctorName = null)
        {
            var context = EnsureContext();
            var query = context.Availabilities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(s => s.DoctorName == doctorName);
            }

            return query
                .Where(s => s.AvailableDate.Date >= DateTime.Today)
                .OrderBy(s => s.AvailableDate)
                .ThenBy(s => s.StartTime)
                .Select(s => new DoctorSlotDto
                {
                    AvailabilityId = s.AvailabilityId,
                    DoctorName = s.DoctorName,
                    AvailableDate = s.AvailableDate,
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.EndTime.ToString(@"hh\:mm")
                })
                .ToList();
        }

        public List<BookableSlotDto> GetBookableSlotOptions(string? doctorName = null)
        {
            var context = EnsureContext();
            var bookedLookup = context.Appointments
                .Where(a => a.Status != "Cancelled")
                .Select(a => $"{a.DoctorName}|{a.RequestedDate:yyyy-MM-dd}|{a.RequestedTime}")
                .ToHashSet();

            var query = context.Availabilities
                .Where(s => s.AvailableDate.Date >= DateTime.Today)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(s => s.DoctorName == doctorName);
            }

            var options = new List<BookableSlotDto>();
            foreach (var slot in query.OrderBy(s => s.AvailableDate).ThenBy(s => s.StartTime).ToList())
            {
                for (var time = slot.StartTime; time < slot.EndTime; time = time.Add(TimeSpan.FromMinutes(30)))
                {
                    var requestedDateTime = slot.AvailableDate.Date.Add(time);
                    if (requestedDateTime <= DateTime.Now)
                    {
                        continue;
                    }

                    var normalizedTime = time.ToString(@"hh\:mm");
                    var key = $"{slot.DoctorName}|{slot.AvailableDate:yyyy-MM-dd}|{normalizedTime}";
                    if (bookedLookup.Contains(key))
                    {
                        continue;
                    }

                    options.Add(new BookableSlotDto
                    {
                        DoctorName = slot.DoctorName,
                        AvailableDate = slot.AvailableDate.Date,
                        RequestedTime = normalizedTime,
                        DisplayLabel = $"{slot.AvailableDate:yyyy-MM-dd} · {normalizedTime}"
                    });
                }
            }

            return options
                .OrderBy(o => o.DoctorName)
                .ThenBy(o => o.AvailableDate)
                .ThenBy(o => o.RequestedTime)
                .ToList();
        }

        public bool TryCancelPatientAppointment(int appointmentId, string patientName)
        {
            var appointment = _appointmentRepository.GetAppointmentById(appointmentId);
            var cancelled = _appointmentRepository.CancelAppointment(appointmentId, patientName);
            if (cancelled && appointment != null)
            {
                _notificationService?.QueueStatusChange(appointment, "Cancelled", appointment.ContactRecipient, appointment.NotificationChannel);
            }

            return cancelled;
        }

        public void CancelPatientAppointment(int appointmentId, string patientName)
        {
            TryCancelPatientAppointment(appointmentId, patientName);
        }

        public bool TryBookNewAppointment(string patientName, string doctorName, DateTime date, string time, string symptoms, string? contactRecipient, string notificationChannel, out Appointment? appointment, out string errorMessage)
        {
            appointment = null;
            if (!TryValidateBookingSlot(doctorName, date, time, null, out errorMessage, out var normalizedTime))
            {
                return false;
            }

            appointment = _appointmentRepository.BookAppointment(patientName, doctorName, date.Date, normalizedTime, symptoms, contactRecipient, notificationChannel);
            _notificationService?.QueueAppointmentConfirmation(appointment, contactRecipient, notificationChannel);
            return true;
        }

        public Appointment BookNewAppointment(string patientName, string doctorName, DateTime date, string time, string symptoms, string? contactRecipient, string notificationChannel)
        {
            if (!TryBookNewAppointment(patientName, doctorName, date, time, symptoms, contactRecipient, notificationChannel, out var appointment, out var errorMessage) || appointment == null)
            {
                throw new InvalidOperationException(errorMessage);
            }

            return appointment;
        }

        public void BookNewAppointment(string patientName, string doctorName, DateTime date, string time, string symptoms)
        {
            BookNewAppointment(patientName, doctorName, date, time, symptoms, null, "InApp");
        }

        public bool ReschedulePatientAppointment(int appointmentId, string patientName, DateTime newDate, string newTime)
        {
            var appointment = _appointmentRepository.GetAppointmentById(appointmentId);
            if (appointment == null || appointment.PatientName != patientName)
            {
                return false;
            }

            if (!TryValidateBookingSlot(appointment.DoctorName, newDate, newTime, appointmentId, out _, out var normalizedTime))
            {
                return false;
            }

            var success = _appointmentRepository.RescheduleAppointment(appointmentId, newDate.Date, normalizedTime);
            if (success)
            {
                appointment.RequestedDate = newDate.Date;
                appointment.RequestedTime = normalizedTime;
                _notificationService?.QueueStatusChange(appointment, "Rescheduled", appointment.ContactRecipient, appointment.NotificationChannel);
            }

            return success;
        }

        private bool TryValidateBookingSlot(string doctorName, DateTime date, string time, int? ignoreAppointmentId, out string errorMessage, out string normalizedTime)
        {
            normalizedTime = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(doctorName))
            {
                errorMessage = "Please select a doctor.";
                return false;
            }

            if (!TimeSpan.TryParse(time, out var requestedTime))
            {
                errorMessage = "Please select a valid time.";
                return false;
            }

            var requestedDateTime = date.Date.Add(requestedTime);
            if (requestedDateTime <= DateTime.Now)
            {
                errorMessage = "Appointments must be scheduled in the future.";
                return false;
            }

            var context = EnsureContext();
            var hasCoverage = context.Availabilities.Any(slot =>
                slot.DoctorName == doctorName &&
                slot.AvailableDate.Date == date.Date &&
                requestedTime >= slot.StartTime &&
                requestedTime < slot.EndTime);

            if (!hasCoverage)
            {
                errorMessage = "This time is outside the doctor's published working hours for that date.";
                return false;
            }

            normalizedTime = requestedTime.ToString(@"hh\:mm");
            var normalizedRequestedTime = normalizedTime;

            var query = context.Appointments.Where(a =>
                a.DoctorName == doctorName &&
                a.RequestedDate.Date == date.Date &&
                a.RequestedTime == normalizedRequestedTime &&
                a.Status != "Cancelled");

            if (ignoreAppointmentId.HasValue)
            {
                query = query.Where(a => a.AppointmentId != ignoreAppointmentId.Value);
            }

            if (query.Any())
            {
                errorMessage = "That appointment time is already taken.";
                return false;
            }

            return true;
        }

        private AppointmentSummaryDto MapSummary(Appointment appointment)
        {
            var context = _context;
            return new AppointmentSummaryDto
            {
                AppointmentId = appointment.AppointmentId,
                PatientName = appointment.PatientName,
                DoctorName = appointment.DoctorName,
                RequestedDate = appointment.RequestedDate,
                RequestedTime = appointment.RequestedTime,
                Symptoms = appointment.Symptoms,
                Status = appointment.Status,
                CanCancel = appointment.Status == "Pending" || appointment.Status == "Approved",
                HasDoctorNotes = context != null && context.DoctorNotes.Any(n => n.AppointmentId == appointment.AppointmentId),
                HasPrescriptions = context != null && context.Prescriptions.Any(p => p.AppointmentId == appointment.AppointmentId)
            };
        }

        private MediNoteDbContext EnsureContext()
        {
            return _context ?? throw new InvalidOperationException("This operation requires a database-backed PatientService instance.");
        }
    }
}

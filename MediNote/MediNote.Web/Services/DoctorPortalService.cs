using System;
using System.Linq;
using MediNote.Web.Contracts;
using MediNote.Web.Data;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    /// <summary>
    /// By: Daniel
    /// 
    /// Doctor workflow service for notes, prescriptions, and reminder actions.
    /// </summary>
    public class DoctorPortalService
    {
        private readonly MediNoteDbContext _context;
        private readonly NotificationService _notificationService;

        public DoctorPortalService(MediNoteDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public AppointmentDetailDto? GetAppointmentDetail(int appointmentId, string? doctorName, bool isAdmin)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment == null)
            {
                return null;
            }

            if (!isAdmin && !string.Equals(appointment.DoctorName, doctorName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return new AppointmentDetailDto
            {
                Appointment = new AppointmentSummaryDto
                {
                    AppointmentId = appointment.AppointmentId,
                    PatientName = appointment.PatientName,
                    DoctorName = appointment.DoctorName,
                    RequestedDate = appointment.RequestedDate,
                    RequestedTime = appointment.RequestedTime,
                    Symptoms = appointment.Symptoms,
                    Status = appointment.Status,
                    CanCancel = appointment.Status == "Pending" || appointment.Status == "Approved",
                    HasDoctorNotes = _context.DoctorNotes.Any(n => n.AppointmentId == appointmentId),
                    HasPrescriptions = _context.Prescriptions.Any(p => p.AppointmentId == appointmentId)
                },
                DoctorNotes = _context.DoctorNotes
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
                Prescriptions = _context.Prescriptions
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
                Notifications = _context.NotificationLogs
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

        public bool AddDoctorNote(int appointmentId, string doctorName, string note, string followUpInstructions, bool isAdmin, out string message)
        {
            var appointment = FindAccessibleAppointment(appointmentId, doctorName, isAdmin);
            if (appointment == null)
            {
                message = "Appointment not found or you do not have permission to update it.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(note))
            {
                message = "Doctor note cannot be empty.";
                return false;
            }

            _context.DoctorNotes.Add(new DoctorNote
            {
                AppointmentId = appointment.AppointmentId,
                DoctorName = appointment.DoctorName,
                Note = note.Trim(),
                FollowUpInstructions = followUpInstructions?.Trim() ?? string.Empty,
                CreatedAtUtc = DateTime.UtcNow
            });
            appointment.LastUpdatedAtUtc = DateTime.UtcNow;
            _context.SaveChanges();

            message = "Doctor note saved successfully.";
            return true;
        }

        public bool AddPrescription(int appointmentId, string doctorName, PrescriptionCreateRequest request, bool isAdmin, out string message)
        {
            var appointment = FindAccessibleAppointment(appointmentId, doctorName, isAdmin);
            if (appointment == null)
            {
                message = "Appointment not found or you do not have permission to update it.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.MedicationName))
            {
                message = "Medication name is required.";
                return false;
            }

            _context.Prescriptions.Add(new Prescription
            {
                AppointmentId = appointment.AppointmentId,
                DoctorName = appointment.DoctorName,
                MedicationName = request.MedicationName.Trim(),
                Dosage = request.Dosage?.Trim() ?? string.Empty,
                Frequency = request.Frequency?.Trim() ?? string.Empty,
                Duration = request.Duration?.Trim() ?? string.Empty,
                Instructions = request.Instructions?.Trim() ?? string.Empty,
                CreatedAtUtc = DateTime.UtcNow
            });
            appointment.LastUpdatedAtUtc = DateTime.UtcNow;
            _context.SaveChanges();

            message = "Prescription saved successfully.";
            return true;
        }

        public bool MarkAppointmentCompleted(int appointmentId, string doctorName, bool isAdmin, out string message)
        {
            var appointment = FindAccessibleAppointment(appointmentId, doctorName, isAdmin);
            if (appointment == null)
            {
                message = "Appointment not found or you do not have permission to update it.";
                return false;
            }

            if (appointment.Status == "Cancelled")
            {
                message = "Cancelled appointments cannot be completed.";
                return false;
            }

            appointment.Status = "Completed";
            appointment.LastUpdatedAtUtc = DateTime.UtcNow;
            _context.SaveChanges();
            _notificationService.QueueStatusChange(appointment, "Completed", appointment.ContactRecipient, appointment.NotificationChannel);

            message = "Appointment marked as completed.";
            return true;
        }

        public bool CancelAppointment(int appointmentId, string doctorName, bool isAdmin, out string message)
        {
            var appointment = FindAccessibleAppointment(appointmentId, doctorName, isAdmin);
            if (appointment == null)
            {
                message = "Appointment not found or you do not have permission to cancel it.";
                return false;
            }

            if (appointment.Status == "Completed")
            {
                message = "Completed appointments cannot be cancelled.";
                return false;
            }

            if (appointment.Status == "Cancelled")
            {
                message = "This appointment is already cancelled.";
                return false;
            }

            appointment.Status = "Cancelled";
            appointment.LastUpdatedAtUtc = DateTime.UtcNow;
            _context.SaveChanges();
            _notificationService.QueueStatusChange(appointment, "Cancelled", appointment.ContactRecipient, appointment.NotificationChannel);

            message = "Appointment cancelled successfully.";
            return true;
        }

        public bool QueueReminder(int appointmentId, string doctorName, string channel, string? recipient, bool isAdmin, out string message)
        {
            var appointment = FindAccessibleAppointment(appointmentId, doctorName, isAdmin);
            if (appointment == null)
            {
                message = "Appointment not found or you do not have permission to send a reminder.";
                return false;
            }

            _notificationService.QueueReminder(appointment, recipient, channel);
            message = "Reminder queued successfully.";
            return true;
        }

        private Appointment? FindAccessibleAppointment(int appointmentId, string doctorName, bool isAdmin)
        {
            return _context.Appointments.FirstOrDefault(a =>
                a.AppointmentId == appointmentId &&
                (isAdmin || a.DoctorName == doctorName));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    /// <summary>
    /// By: Camila Esguerra
    /// Stores appointment confirmations and reminders as queued notification logs.
    /// </summary>
    public class NotificationService
    {
        private readonly MediNoteDbContext _context;

        public NotificationService(MediNoteDbContext context)
        {
            _context = context;
        }

        // Core method to create a notification log entry in the database.
        public NotificationLog Queue(int appointmentId, string patientName, string? recipient, string channel, string type, string message)
        {
            var log = new NotificationLog
            {
                AppointmentId = appointmentId,
                PatientName = patientName,
                Recipient = recipient ?? string.Empty,
                Channel = string.IsNullOrWhiteSpace(channel) ? "InApp" : channel,
                Type = type,
                Message = message,
                Status = "Queued",
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.NotificationLogs.Add(log);
            _context.SaveChanges();
            return log;
        }

        
        public NotificationLog QueueAppointmentConfirmation(Appointment appointment, string? recipient = null, string channel = "InApp")
        {
            return Queue(
                appointment.AppointmentId,
                appointment.PatientName,
                ChooseRecipient(appointment, recipient),
                channel,
                "Confirmation",
                $"Your appointment request with {appointment.DoctorName} on {appointment.RequestedDate:yyyy-MM-dd} at {appointment.RequestedTime} has been received.");
        }

        // Method to queue a status change notification for an appointment. It constructs a message based on the new status and relevant appointment details.
        public NotificationLog QueueStatusChange(Appointment appointment, string status, string? recipient = null, string channel = "InApp")
        {
            return Queue(
                appointment.AppointmentId,
                appointment.PatientName,
                ChooseRecipient(appointment, recipient),
                channel,
                "StatusUpdate",
                $"Appointment #{appointment.AppointmentId} is now {status}. Doctor: {appointment.DoctorName}. Date: {appointment.RequestedDate:yyyy-MM-dd} {appointment.RequestedTime}.");
        }

        // Method to queue a reminder notification for an upcoming appointment. It constructs a message reminding the patient of their scheduled appointment details.
        public NotificationLog QueueReminder(Appointment appointment, string? recipient = null, string channel = "InApp")
        {
            return Queue(
                appointment.AppointmentId,
                appointment.PatientName,
                ChooseRecipient(appointment, recipient),
                channel,
                "Reminder",
                $"Reminder: you have an upcoming appointment with {appointment.DoctorName} on {appointment.RequestedDate:yyyy-MM-dd} at {appointment.RequestedTime}.");
        }

        // Method to retrieve the notification logs for a specific appointment, ordered by creation time in descending order.
        public List<NotificationLog> GetLogsForAppointment(int appointmentId)
        {
            return _context.NotificationLogs
                .Where(n => n.AppointmentId == appointmentId)
                .OrderByDescending(n => n.CreatedAtUtc)
                .ToList();
        }

        //  Helper method to determine the recipient of a notification. It prioritizes an explicitly provided recipient, then checks for a contact recipient in the appointment, and finally defaults to the patient's name if no other recipient is specified.
        private static string ChooseRecipient(Appointment appointment, string? explicitRecipient)
        {
            if (!string.IsNullOrWhiteSpace(explicitRecipient))
            {
                return explicitRecipient;
            }

            if (!string.IsNullOrWhiteSpace(appointment.ContactRecipient))
            {
                return appointment.ContactRecipient;
            }

            return appointment.PatientName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediNote.Web.Contracts
{
    public class RegisterAccountRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Patient";

        public string SecurityId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class BookAppointmentRequest
    {
        public string? PatientName { get; set; }

        [Required]
        public string DoctorName { get; set; } = string.Empty;

        [Required]
        public DateTime RequestedDate { get; set; }

        [Required]
        public string RequestedTime { get; set; } = string.Empty;

        [Required]
        public string Symptoms { get; set; } = string.Empty;

        public string? ContactRecipient { get; set; }
        public string NotificationChannel { get; set; } = "InApp";
    }

    public class UpdateAppointmentStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    public class RescheduleAppointmentRequest
    {
        [Required]
        public DateTime NewDate { get; set; }

        [Required]
        public string NewTime { get; set; } = string.Empty;
    }

    public class DoctorNoteCreateRequest
    {
        [Required]
        public string Note { get; set; } = string.Empty;
        public string FollowUpInstructions { get; set; } = string.Empty;
    }

    public class PrescriptionCreateRequest
    {
        [Required]
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }

    public class QueueReminderRequest
    {
        public string NotificationChannel { get; set; } = "InApp";
        public string? Recipient { get; set; }
    }

    public class AppointmentSummaryDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string RequestedTime { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool CanCancel { get; set; }
        public bool HasDoctorNotes { get; set; }
        public bool HasPrescriptions { get; set; }
    }

    public class DoctorSlotDto
    {
        public int AvailabilityId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AvailableDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }

    public class BookableSlotDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AvailableDate { get; set; }
        public string RequestedTime { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
    }

    public class DoctorNoteDto
    {
        public int DoctorNoteId { get; set; }
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string FollowUpInstructions { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }

    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }

    public class NotificationLogDto
    {
        public int NotificationLogId { get; set; }
        public int AppointmentId { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }

    public class AppointmentDetailDto
    {
        public AppointmentSummaryDto Appointment { get; set; } = new();
        public List<DoctorNoteDto> DoctorNotes { get; set; } = new();
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
        public List<NotificationLogDto> Notifications { get; set; } = new();
    }

    public class PatientDashboardDto
    {
        public string PatientName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public AppointmentSummaryDto? NextAppointment { get; set; }
        public List<AppointmentSummaryDto> Appointments { get; set; } = new();
    }

    public class AdminDashboardSummaryDto
    {
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public List<AppointmentSummaryDto> RecentAppointments { get; set; } = new();
    }
}

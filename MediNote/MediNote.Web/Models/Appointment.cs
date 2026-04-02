using System;

namespace MediNote.Web.Models
{
    /// <summary>
    /// Represents an appointment request made by a patient to see a doctor.
    /// </summary>
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string RequestedTime { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Cancelled, Completed
        public string ContactRecipient { get; set; } = string.Empty;
        public string NotificationChannel { get; set; } = "InApp";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

using System;

namespace MediNote.Web.Models
{
    /// <summary>
    /// By: Daniel
    /// 
    /// Internal notification log used for confirmation and reminder tracking.
    /// This can later be connected to email or SMS providers.
    /// </summary>
    public class NotificationLog
    {
        public int NotificationLogId { get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Channel { get; set; } = "InApp";
        public string Type { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "Queued";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

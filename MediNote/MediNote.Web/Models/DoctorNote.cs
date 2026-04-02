using System;

namespace MediNote.Web.Models
{
    /// <summary>
    /// Clinical note written by a doctor for a completed or ongoing appointment.
    /// </summary>
    public class DoctorNote
    {
        public int DoctorNoteId { get; set; }
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string FollowUpInstructions { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

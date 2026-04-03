using System;
using System.Collections.Generic;

namespace MediNote.Web.ViewModels
{
    /// <summary>
    /// By: Daniel
    /// View model used by the Pending Appointments page.
    /// </summary>
    public class PendingAppointmentsViewModel
    {
        public string StatusMessage { get; set; } = string.Empty;
        public List<PendingAppointmentItemViewModel> PendingAppointments { get; set; } = new();
    }

    /// <summary>
    /// Represents one pending appointment row.
    /// </summary>
    public class PendingAppointmentItemViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string RequestedTime { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Priority { get; set; } = "Unknown";
        public string Status { get; set; } = string.Empty;
    }
}

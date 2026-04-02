using System;
using System.Collections.Generic;

namespace MediNote.Web.ViewModels
{
    /// <summary>
    /// View model used to display the doctor's schedule page.
    /// </summary>
    public class DoctorScheduleViewModel
    {
        public string DoctorName { get; set; } = string.Empty;
        public DateTime ScheduleDate { get; set; }
        public List<DoctorScheduleItemViewModel> Appointments { get; set; } = new();
    }

    public class DoctorScheduleItemViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; } = string.Empty;
        public string Priority { get; set; } = "Unknown";
        public string Status { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
    }
}

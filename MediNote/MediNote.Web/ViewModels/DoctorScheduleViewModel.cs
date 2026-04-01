using System;
using System.Collections.Generic;

namespace MediNote.Web.ViewModels
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// View model used to display the doctor's schedule page.
    /// </summary>
    public class DoctorScheduleViewModel
    {
        /// <summary>
        /// Gets or sets the doctor's display name.
        /// </summary>
        public string DoctorName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date for the displayed schedule.
        /// </summary>
        public DateTime ScheduleDate { get; set; }

        /// <summary>
        /// Gets or sets the list of appointments shown on the page.
        /// </summary>
        public List<DoctorScheduleItemViewModel> Appointments { get; set; } = new();
    }

    /// <summary>
    /// Author: Daniel Guillaumont
    /// Represents one appointment row on the doctor's schedule page.
    /// </summary>
    public class DoctorScheduleItemViewModel
    {
        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the assigned doctor name.
        /// </summary>
        public string DoctorName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the appointment date.
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Gets or sets the appointment time as display text.
        /// </summary>
        public string AppointmentTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority of the appointment.
        /// </summary>
        public string Priority { get; set; } = "Unknown";

        /// <summary>
        /// Gets or sets the appointment status.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
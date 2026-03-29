using System;
using System.ComponentModel.DataAnnotations;

namespace MediNote.Web.ViewModels
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// View model used by the Reschedule Appointment page.
    /// </summary>
    public class RescheduleAppointmentViewModel
    {
        /// <summary>
        /// Gets or sets the appointment ID.
        /// </summary>
        public int AppointmentId { get; set; }

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current appointment date.
        /// </summary>
        public DateTime CurrentDate { get; set; }

        /// <summary>
        /// Gets or sets the current appointment time.
        /// </summary>
        public string CurrentTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new selected appointment date.
        /// </summary>
        [Required(ErrorMessage = "New date is required.")]
        [DataType(DataType.Date)]
        public DateTime? NewDate { get; set; }

        /// <summary>
        /// Gets or sets the new selected appointment time.
        /// </summary>
        [Required(ErrorMessage = "New time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan? NewTime { get; set; }

        /// <summary>
        /// Gets or sets the status message shown after a successful reschedule.
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;
    }
}
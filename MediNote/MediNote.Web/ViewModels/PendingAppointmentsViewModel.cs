using System;
using System.Collections.Generic;

namespace MediNote.Web.ViewModels
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// View model used by the Pending Appointments page.
    /// </summary>
    public class PendingAppointmentsViewModel
    {
        /// <summary>
        /// Gets or sets the status message shown after approving or rejecting.
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of pending appointments.
        /// </summary>
        public List<PendingAppointmentItemViewModel> PendingAppointments { get; set; } = new();
    }

    /// <summary>
    /// Author: Daniel Guillaumont
    /// Represents one pending appointment row.
    /// </summary>
    public class PendingAppointmentItemViewModel
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
        /// Gets or sets the doctor name.
        /// </summary>
        public string DoctorName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the requested date.
        /// </summary>
        public DateTime RequestedDate { get; set; }

        /// <summary>
        /// Gets or sets the symptoms entered by the patient.
        /// </summary>
        public string Symptoms { get; set; } = string.Empty;
    }
}
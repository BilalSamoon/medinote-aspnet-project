using System;

namespace MediNote.Web.Models
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Represents one doctor availability slot.
    /// </summary>
    public class Availability
    {
        /// <summary>
        /// Gets or sets the availability ID.
        /// </summary>
        public int AvailabilityId { get; set; }

        /// <summary>
        /// Gets or sets the doctor name.
        /// </summary>
        public string DoctorName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the available date.
        /// </summary>
        public DateTime AvailableDate { get; set; }

        /// <summary>
        /// Gets or sets the slot start time.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Gets or sets the slot end time.
        /// </summary>
        public TimeSpan EndTime { get; set; }
    }
}
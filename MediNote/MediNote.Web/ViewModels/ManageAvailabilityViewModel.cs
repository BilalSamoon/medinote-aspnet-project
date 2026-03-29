using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediNote.Web.Models;

namespace MediNote.Web.ViewModels
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// View model used by the Manage Availability page.
    /// </summary>
    public class ManageAvailabilityViewModel
    {
        /// <summary>
        /// Gets or sets the selected available date.
        /// </summary>
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime? AvailableDate { get; set; }

        /// <summary>
        /// Gets or sets the selected start time.
        /// </summary>
        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the selected end time.
        /// </summary>
        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the success message shown after saving.
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the existing availability slots.
        /// </summary>
        public List<Availability> ExistingSlots { get; set; } = new();
    }
}
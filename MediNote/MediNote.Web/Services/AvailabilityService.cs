using System;
using System.Collections.Generic;
using MediNote.Web.Models;
using MediNote.Web.ViewModels;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Provides business logic for doctor availability.
    /// </summary>
    public class AvailabilityService
    {
        /// <summary>
        /// Returns the manage availability page model with sample data.
        /// </summary>
        /// <returns>A populated manage availability view model.</returns>
        public ManageAvailabilityViewModel GetManageAvailabilityViewModel()
        {
            return new ManageAvailabilityViewModel
            {
                ExistingSlots = GetSampleAvailabilitySlots()
            };
        }

        /// <summary>
        /// Returns sample availability slots for display.
        /// </summary>
        /// <returns>A list of sample availability slots.</returns>
        public List<Availability> GetSampleAvailabilitySlots()
        {
            return new List<Availability>
            {
                new Availability
                {
                    AvailabilityId = 1,
                    DoctorName = "Dr. Daniel Guillaumont",
                    AvailableDate = new DateTime(2026, 3, 30),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0)
                },
                new Availability
                {
                    AvailabilityId = 2,
                    DoctorName = "Dr. Daniel Guillaumont",
                    AvailableDate = new DateTime(2026, 3, 31),
                    StartTime = new TimeSpan(1, 0, 0),
                    EndTime = new TimeSpan(4, 0, 0)
                }
            };
        }

        /// <summary>
        /// Checks whether the end time is after the start time.
        /// </summary>
        /// <param name="startTime">The selected start time.</param>
        /// <param name="endTime">The selected end time.</param>
        /// <returns>True if end time is after start time; otherwise false.</returns>
        public bool IsEndTimeAfterStartTime(TimeSpan startTime, TimeSpan endTime)
        {
            return endTime > startTime;
        }

        /// <summary>
        /// Checks whether the proposed slot overlaps with an existing sample slot on the same date.
        /// </summary>
        /// <param name="availableDate">The selected date.</param>
        /// <param name="startTime">The selected start time.</param>
        /// <param name="endTime">The selected end time.</param>
        /// <returns>True if overlap exists; otherwise false.</returns>
        public bool HasOverlappingSlot(DateTime availableDate, TimeSpan startTime, TimeSpan endTime)
        {
            var existingSlots = GetSampleAvailabilitySlots();

            foreach (var slot in existingSlots)
            {
                if (slot.AvailableDate.Date == availableDate.Date)
                {
                    bool overlaps = startTime < slot.EndTime && endTime > slot.StartTime;
                    if (overlaps)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
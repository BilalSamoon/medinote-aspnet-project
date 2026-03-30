using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
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
        private readonly MediNoteDbContext _context;

        public AvailabilityService(MediNoteDbContext context)
        {
            _context = context;
        }

        public AvailabilityService()
        {
        }

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
        /// Returns availability slots for display.
        /// </summary>
        /// <returns>A list of availability slots.</returns>
        public List<Availability> GetSampleAvailabilitySlots()
        {
            return _context.Availabilities.ToList();
        }

        public void AddAvailability(Availability availability)
        {
            _context.Availabilities.Add(availability);
            _context.SaveChanges();
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
using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;
using MediNote.Web.ViewModels;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Provides business logic for doctor availability.
    /// </summary>
    public class AvailabilityService
    {
        private readonly MediNoteDbContext _context;

        public AvailabilityService(MediNoteDbContext context)
        {
            _context = context;
        }

        public ManageAvailabilityViewModel GetManageAvailabilityViewModel(string? doctorName = null, bool isAdmin = false)
        {
            return new ManageAvailabilityViewModel
            {
                ExistingSlots = GetSampleAvailabilitySlots(doctorName, isAdmin)
            };
        }

        public List<Availability> GetSampleAvailabilitySlots(string? doctorName = null, bool isAdmin = true)
        {
            var query = _context.Availabilities.AsQueryable();

            if (!isAdmin && !string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            return query
                .OrderBy(a => a.AvailableDate)
                .ThenBy(a => a.StartTime)
                .ToList();
        }

        public void AddAvailability(Availability availability)
        {
            _context.Availabilities.Add(availability);
            _context.SaveChanges();
        }

        public bool IsEndTimeAfterStartTime(TimeSpan startTime, TimeSpan endTime)
        {
            return endTime > startTime;
        }

        public bool HasOverlappingSlot(DateTime availableDate, TimeSpan startTime, TimeSpan endTime)
        {
            return HasOverlappingSlot(availableDate, startTime, endTime, null);
        }

        public bool HasOverlappingSlot(DateTime availableDate, TimeSpan startTime, TimeSpan endTime, string? doctorName)
        {
            var existingSlots = _context.Availabilities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                existingSlots = existingSlots.Where(slot => slot.DoctorName == doctorName);
            }

            return existingSlots.Any(slot =>
                slot.AvailableDate.Date == availableDate.Date &&
                startTime < slot.EndTime &&
                endTime > slot.StartTime);
        }
    }
}

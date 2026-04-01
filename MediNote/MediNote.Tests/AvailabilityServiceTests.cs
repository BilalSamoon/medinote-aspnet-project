using System;
using MediNote.Web.Services;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;
using MediNote.Web.Models;

namespace MediNote.Tests
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// NUnit tests for AvailabilityService.
    /// </summary>
    public class AvailabilityServiceTests
    {
        private MediNoteDbContext _context = null!;
        private AvailabilityService _availabilityService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MediNoteDbContext(options);
            _availabilityService = new AvailabilityService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void IsEndTimeAfterStartTime_ReturnsTrue_WhenEndIsLater()
        {
            var result = _availabilityService.IsEndTimeAfterStartTime(
                new TimeSpan(9, 0, 0),
                new TimeSpan(11, 0, 0));

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEndTimeAfterStartTime_ReturnsFalse_WhenEndIsEarlier()
        {
            var result = _availabilityService.IsEndTimeAfterStartTime(
                new TimeSpan(9, 0, 0),
                new TimeSpan(8, 0, 0));

            Assert.That(result, Is.False);
        }

        [Test]
        public void HasOverlappingSlot_ReturnsTrue_ForConflictingSlot()
        {
            // Arrange: Add an existing availability slot that overlaps with the test slot
            var existingSlot = new Availability
            {
                AvailabilityId = 1,
                DoctorName = "Dr. Test",
                AvailableDate = new DateTime(2026, 3, 30),
                StartTime = new TimeSpan(9, 30, 0),  // 9:30 AM
                EndTime = new TimeSpan(10, 30, 0)     // 10:30 AM - overlaps with 10:00-11:00
            };
            _context.Availabilities.Add(existingSlot);
            _context.SaveChanges();

            // Act: Check for overlap with 10:00-11:00 slot
            var result = _availabilityService.HasOverlappingSlot(
                new DateTime(2026, 3, 30),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0));

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasOverlappingSlot_ReturnsFalse_ForNonConflictingSlot()
        {
            // Arrange: Add an existing availability slot that does NOT overlap
            var existingSlot = new Availability
            {
                AvailabilityId = 1,
                DoctorName = "Dr. Test",
                AvailableDate = new DateTime(2026, 3, 30),
                StartTime = new TimeSpan(9, 0, 0),    // 9:00 AM
                EndTime = new TimeSpan(10, 0, 0)      // 10:00 AM - does not overlap with 10:00-11:00 on a different date
            };
            _context.Availabilities.Add(existingSlot);
            _context.SaveChanges();

            // Act: Check for overlap with 10:00-11:00 slot on a different date
            var result = _availabilityService.HasOverlappingSlot(
                new DateTime(2026, 4, 2),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0));

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
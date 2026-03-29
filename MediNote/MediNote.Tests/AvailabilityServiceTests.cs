using System;
using MediNote.Web.Services;
using NUnit.Framework;

namespace MediNote.Tests
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// NUnit tests for AvailabilityService.
    /// </summary>
    public class AvailabilityServiceTests
    {
        private AvailabilityService _availabilityService = null!;

        [SetUp]
        public void Setup()
        {
            _availabilityService = new AvailabilityService();
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
            var result = _availabilityService.HasOverlappingSlot(
                new DateTime(2026, 3, 30),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0));

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasOverlappingSlot_ReturnsFalse_ForNonConflictingSlot()
        {
            var result = _availabilityService.HasOverlappingSlot(
                new DateTime(2026, 4, 2),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0));

            Assert.That(result, Is.False);
        }
    }
}
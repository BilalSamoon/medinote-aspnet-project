using System;
using MediNote.Web.Services;
using NUnit.Framework;

namespace MediNote.Tests
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// NUnit tests for DoctorAppointmentService.
    /// </summary>
    public class DoctorAppointmentServiceTests
    {
        private DoctorAppointmentService _doctorAppointmentService = null!;

        [SetUp]
        public void Setup()
        {
            _doctorAppointmentService = new DoctorAppointmentService();
        }

        [Test]
        public void ApproveAppointment_ReturnsCorrectMessage()
        {
            var result = _doctorAppointmentService.ApproveAppointment(1);

            Assert.That(result, Is.EqualTo("Appointment #1 was approved successfully."));
        }

        [Test]
        public void RejectAppointment_ReturnsCorrectMessage()
        {
            var result = _doctorAppointmentService.RejectAppointment(2);

            Assert.That(result, Is.EqualTo("Appointment #2 was rejected successfully."));
        }

        [Test]
        public void IsAppointmentEligibleForReschedule_ReturnsTrue_ForKnownAppointment()
        {
            var result = _doctorAppointmentService.IsAppointmentEligibleForReschedule(1);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsAppointmentEligibleForReschedule_ReturnsFalse_ForUnknownAppointment()
        {
            var result = _doctorAppointmentService.IsAppointmentEligibleForReschedule(99);

            Assert.That(result, Is.False);
        }

        [Test]
        public void HasRescheduleConflict_ReturnsTrue_ForBookedSlot()
        {
            var result = _doctorAppointmentService.HasRescheduleConflict(
                new DateTime(2026, 3, 29),
                new TimeSpan(9, 0, 0));

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasRescheduleConflict_ReturnsFalse_ForAvailableSlot()
        {
            var result = _doctorAppointmentService.HasRescheduleConflict(
                new DateTime(2026, 4, 2),
                new TimeSpan(10, 30, 0));

            Assert.That(result, Is.False);
        }
    }
}
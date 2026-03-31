using System;
using MediNote.Web.Services;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;

namespace MediNote.Tests
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// NUnit tests for DoctorAppointmentService.
    /// </summary>
    public class DoctorAppointmentServiceTests
    {
        private MediNoteDbContext _context = null!;
        private AppointmentRepository _appointmentRepository = null!;
        private DoctorAppointmentService _doctorAppointmentService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MediNoteDbContext(options);
            _appointmentRepository = new AppointmentRepository(_context);
            _doctorAppointmentService = new DoctorAppointmentService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void ApproveAppointment_ReturnsCorrectMessage()
        {
            // Arrange: Create an appointment to approve
            _appointmentRepository.BookAppointment("John Doe", "Dr. Smith", DateTime.Now, "10:00 AM", "Headache");
            
            // Act
            var result = _doctorAppointmentService.ApproveAppointment(1);

            // Assert
            Assert.That(result, Is.EqualTo("Appointment #1 was approved successfully."));
        }

        [Test]
        public void RejectAppointment_ReturnsCorrectMessage()
        {
            // Arrange: Create appointments (ID 1 and 2)
            _appointmentRepository.BookAppointment("John Doe", "Dr. Smith", DateTime.Now, "10:00 AM", "Headache");
            _appointmentRepository.BookAppointment("Jane Smith", "Dr. Jones", DateTime.Now, "11:00 AM", "Fever");
            
            // Act
            var result = _doctorAppointmentService.RejectAppointment(2);

            // Assert
            Assert.That(result, Is.EqualTo("Appointment #2 was rejected successfully."));
        }

        [Test]
        public void IsAppointmentEligibleForReschedule_ReturnsTrue_ForKnownAppointment()
        {
            // Arrange: Create an appointment to check reschedule eligibility
            _appointmentRepository.BookAppointment("John Doe", "Dr. Smith", DateTime.Now, "10:00 AM", "Headache");
            
            // Act
            var result = _doctorAppointmentService.IsAppointmentEligibleForReschedule(1);

            // Assert
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
            // Arrange: Create an appointment at the target date and time
            _appointmentRepository.BookAppointment("John Doe", "Dr. Smith", new DateTime(2026, 3, 29), "09:00", "Headache");

            // Act
            var result = _doctorAppointmentService.HasRescheduleConflict(
                new DateTime(2026, 3, 29),
                new TimeSpan(9, 0, 0));

            // Assert
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
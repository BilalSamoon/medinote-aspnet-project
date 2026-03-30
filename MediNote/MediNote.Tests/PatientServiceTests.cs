using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;
using MediNote.Web.Services;
using System;

/// <summary>
/// Author: Camila Esguerra
/// NUnit tests for PatientService, which manages patient interactions like appointment cancellation.
/// </summary>
/// 
namespace MediNote.Tests
{
    public class PatientServiceTests
    {
        private MediNoteDbContext _context;
        private AppointmentRepository _repository;
        private PatientService _patientService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MediNoteDbContext(options);
            _repository = new AppointmentRepository(_context);
            _patientService = new PatientService(_repository);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void CancelPatientAppointment_ShouldChangeStatusToCancelled()
        {
            // Arrange
            var appointment = _repository.BookAppointment("Jane Doe", "Dr. Adams", DateTime.Now, "11:00 AM", "Cough");

            // Act
            _patientService.CancelPatientAppointment(appointment.AppointmentId, "Jane Doe");

            // Assert
            var cancelledAppointment = _repository.GetAppointmentById(appointment.AppointmentId);
            Assert.That(cancelledAppointment.Status, Is.EqualTo("Cancelled"));
        }
    }
}
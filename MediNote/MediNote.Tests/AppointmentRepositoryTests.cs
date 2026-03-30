using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;
using MediNote.Web.Services;
using MediNote.Web.Models;
using System;
using System.Linq;

/// <summary>
/// Author: Camila Esguerra
/// NUnit tests for AppointmentRepository, which handles appointment booking and retrieval.
/// </summary>

namespace MediNote.Tests
{
    public class AppointmentRepositoryTests
    {
        private MediNoteDbContext _context;
        private AppointmentRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Configure an In-Memory database for testing
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique DB for each test
                .Options;

            _context = new MediNoteDbContext(options);
            _repository = new AppointmentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void BookAppointment_ShouldAddAppointmentToDatabase()
        {
            // Act
            _repository.BookAppointment("John Doe", "Dr. Smith", DateTime.Now, "10:00 AM", "Headache");

            // Assert
            Assert.That(_context.Appointments.Count(), Is.EqualTo(1));
            Assert.That(_context.Appointments.First().PatientName, Is.EqualTo("John Doe"));
        }
    }
}
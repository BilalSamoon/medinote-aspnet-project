using MediNote.Web.Services;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;
using System;
using MediNote.Web.Models;

namespace MediNote.Tests
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// NUnit tests for ScheduleService.
    /// </summary>
    public class ScheduleServiceTests
    {
        private MediNoteDbContext _context = null!;
        private AppointmentRepository _appointmentRepository = null!;
        private ScheduleService _scheduleService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new MediNoteDbContext(options);
            _context = context;

            _context.Appointments.AddRange(
                new Appointment { PatientName = "John Doe", DoctorName = "Dr. Daniel Guillaumont", RequestedDate = DateTime.Now, RequestedTime = "10:00 AM" },
                new Appointment { PatientName = "Jane Smith", DoctorName = "Dr. Daniel Guillaumont", RequestedDate = DateTime.Now, RequestedTime = "11:00 AM" },
                new Appointment { PatientName = "Bob Johnson", DoctorName = "Dr. Daniel Guillaumont", RequestedDate = DateTime.Now, RequestedTime = "12:00 PM" }
            );
            _context.SaveChanges();

            var priorityCalculationService = new PriorityCalculationService();
            _scheduleService = new ScheduleService(context, priorityCalculationService);
            _appointmentRepository = new AppointmentRepository(context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void GetDoctorSchedule_ReturnsModel()
        {
            var result = _scheduleService.GetDoctorSchedule();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetDoctorSchedule_ReturnsExpectedDoctorName()
        {
            var result = _scheduleService.GetDoctorSchedule("Dr. Daniel Guillaumont");

            Assert.That(result.DoctorName, Is.EqualTo("Dr. Daniel Guillaumont"));
        }

        [Test]
        public void GetDoctorSchedule_ReturnsThreeAppointments()
        {
            var result = _scheduleService.GetDoctorSchedule();

            Assert.That(result.Appointments.Count, Is.EqualTo(3));
        }
    }
}
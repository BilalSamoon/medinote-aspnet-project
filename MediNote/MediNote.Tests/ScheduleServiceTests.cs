using MediNote.Web.Services;
using NUnit.Framework;

namespace MediNote.Tests
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// NUnit tests for ScheduleService.
    /// </summary>
    public class ScheduleServiceTests
    {
        private ScheduleService _scheduleService = null!;

        [SetUp]
        public void Setup()
        {
            _scheduleService = new ScheduleService();
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
            var result = _scheduleService.GetDoctorSchedule();

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
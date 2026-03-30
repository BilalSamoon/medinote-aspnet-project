using MediNote.Web.Services;
using NUnit.Framework;

namespace MediNote.Tests
{
    /// Author: Bilal Ahmed Samoon
    /// NUnit tests for AdminReportService.
    public class AdminReportServiceTests
    {
        private AdminReportService _service = null!;

        [SetUp]
        public void Setup()
        {
            _service = new AdminReportService();
        }

        [Test]
        public void GetTotalAppointments_ReturnsCorrectValue()
        {
            var result = _service.GetTotalAppointments(10);

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetPendingAppointments_ReturnsCorrectValue()
        {
            var result = _service.GetPendingAppointments(5);

            Assert.That(result, Is.EqualTo(5));
        }
    }
}
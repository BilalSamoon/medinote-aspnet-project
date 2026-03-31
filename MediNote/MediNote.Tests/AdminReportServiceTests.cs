using MediNote.Web.Services;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;
using System;

namespace MediNote.Tests
{
    /// Author: Bilal Ahmed Samoon
    /// NUnit tests for AdminReportService.
    public class AdminReportServiceTests
    {
        private MediNoteDbContext _context = null!;
        private AdminReportService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MediNoteDbContext(options);
            _service = new AdminReportService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
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
using MediNote.Web.Services;
using NUnit.Framework;

namespace MediNote.Tests
{
    /// Author: Bilal Ahmed Samoon
    /// NUnit tests for PriorityCalculationService.
    public class PriorityCalculationServiceTests
    {
        private PriorityCalculationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _service = new PriorityCalculationService();
        }

        [Test]
        public void GetPriority_ReturnsHigh_ForChestPain()
        {
            var result = _service.GetPriority("chest pain");

            Assert.That(result, Is.EqualTo("HIGH"));
        }

        [Test]
        public void GetPriority_ReturnsMedium_ForFever()
        {
            var result = _service.GetPriority("fever");

            Assert.That(result, Is.EqualTo("MEDIUM"));
        }

        [Test]
        public void GetPriority_ReturnsLow_ForNormalSymptoms()
        {
            var result = _service.GetPriority("headache");

            Assert.That(result, Is.EqualTo("LOW"));
        }
    }
}
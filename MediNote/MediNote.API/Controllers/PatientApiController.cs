using MediNote.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.API.Controllers
{
    /// <summary>
    /// Author: Bilal Ahmed Samoon
    /// Controller responsible for handling patient-related API operations including
    /// booking appointments, retrieving appointment history, and cancelling appointments.
    /// </summary>
    [ApiController]
    [Route("api/patient")]
    public class PatientApiController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientApiController(PatientService patientService)
        {
            _patientService = patientService;
        }

        //Get all appointments for a patient
        [HttpGet("appointments/{name}")]
        public IActionResult GetAppointments(string name)
        {
            var appointments = _patientService.GetPatientAppointments(name);
            return Ok(appointments);
        }

        // Book new appointment
        [HttpPost("book")]
        public IActionResult BookAppointment(
            [FromQuery] string patientName,
            [FromQuery] string doctorName,
            [FromQuery] DateTime date,
            [FromQuery] string time,
            [FromQuery] string symptoms)
        {
            _patientService.BookNewAppointment(patientName, doctorName, date, time, symptoms);
            return Ok("Appointment booked successfully.");
        }

        // Cancel appointment
        [HttpPost("cancel/{id}")]
        public IActionResult CancelAppointment(int id, [FromQuery] string patientName)
        {
            _patientService.CancelPatientAppointment(id, patientName);
            return Ok("Appointment cancelled successfully.");
        }
    }
}
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
            var appointments = _patientService.GetPatientDashboard(name);
            return Ok(appointments);
        }

        // Book new appointment
        [HttpPost("book")]
        public IActionResult BookAppointment([FromBody] MediNote.Web.Contracts.BookAppointmentRequest request)
        {
            if (string.IsNullOrEmpty(request.PatientName) || string.IsNullOrEmpty(request.DoctorName))
            {
                return BadRequest("PatientName and DoctorName are required.");
            }

            _patientService.BookNewAppointment(request.PatientName, request.DoctorName, request.RequestedDate, request.RequestedTime, request.Symptoms);
            return Ok("Appointment booked successfully.");
        }

        // Cancel appointment
        [HttpPost("cancel/{id}")]
        public IActionResult CancelAppointment(int id, [FromBody] string patientName)
        {
            if (string.IsNullOrEmpty(patientName)) return BadRequest();
            _patientService.CancelPatientAppointment(id, patientName);
            return Ok("Appointment cancelled successfully.");
        }
    }
}
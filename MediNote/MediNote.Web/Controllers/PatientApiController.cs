using System.Linq;
using MediNote.Web.Contracts;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    [ApiController]
    [Authorize(Roles = "Patient,Admin")]
    [Route("api/patient")]
    public class PatientApiController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly UserRepository _userRepository;

        public PatientApiController(PatientService patientService, UserRepository userRepository)
        {
            _patientService = patientService;
            _userRepository = userRepository;
        }

        [HttpGet("dashboard")]
        public ActionResult<PatientDashboardDto> GetDashboard()
        {
            var patientName = User.Identity?.Name ?? string.Empty;
            return Ok(_patientService.GetPatientDashboard(patientName));
        }

        [HttpGet("appointments")]
        public IActionResult GetAppointments()
        {
            var dashboard = _patientService.GetPatientDashboard(User.Identity?.Name ?? string.Empty);
            return Ok(dashboard.Appointments);
        }

        [HttpGet("appointments/{id:int}")]
        public IActionResult GetAppointmentDetail(int id)
        {
            var detail = _patientService.GetAppointmentDetail(id, User.Identity?.Name ?? string.Empty, User.IsInRole("Admin"));
            return detail == null ? NotFound() : Ok(detail);
        }

        [HttpPost("appointments")]
        public IActionResult BookAppointment([FromBody] BookAppointmentRequest request)
        {
            var patientName = User.IsInRole("Admin") && !string.IsNullOrWhiteSpace(request.PatientName)
                ? request.PatientName!
                : User.Identity?.Name ?? string.Empty;

            var ok = _patientService.TryBookNewAppointment(
                patientName,
                request.DoctorName,
                request.RequestedDate,
                request.RequestedTime,
                request.Symptoms,
                request.ContactRecipient,
                request.NotificationChannel,
                out var appointment,
                out var errorMessage);

            if (!ok || appointment == null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Appointment booked successfully.", appointmentId = appointment.AppointmentId });
        }

        [HttpPost("appointments/{id:int}/cancel")]
        public IActionResult CancelAppointment(int id)
        {
            var patientName = User.Identity?.Name ?? string.Empty;
            var success = _patientService.TryCancelPatientAppointment(id, patientName);
            return success ? Ok(new { message = "Appointment cancelled successfully." }) : BadRequest(new { message = "Unable to cancel the appointment." });
        }

        [HttpPost("appointments/{id:int}/reschedule")]
        public IActionResult RescheduleAppointment(int id, [FromBody] RescheduleAppointmentRequest request)
        {
            var success = _patientService.ReschedulePatientAppointment(id, User.Identity?.Name ?? string.Empty, request.NewDate, request.NewTime);
            return success ? Ok(new { message = "Appointment rescheduled successfully." }) : BadRequest(new { message = "Unable to reschedule the appointment." });
        }

        [HttpGet("doctor-slots")]
        public IActionResult GetDoctorSlots([FromQuery] string? doctorName = null)
        {
            return Ok(_patientService.GetDoctorSlots(doctorName));
        }

        [HttpGet("bookable-slots")]
        public IActionResult GetBookableSlots([FromQuery] string? doctorName = null)
        {
            return Ok(_patientService.GetBookableSlotOptions(doctorName));
        }

        [HttpGet("doctors")]
        public IActionResult GetDoctors()
        {
            var doctors = _userRepository.GetDoctors()
                .Select(d => new { name = d.DisplayName, doctorId = d.SecurityId, email = d.Email })
                .ToList();
            return Ok(doctors);
        }
    }
}

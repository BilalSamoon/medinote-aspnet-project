using System.Linq;
using MediNote.Web.Contracts;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{

    //By: Camila Esguerra
    // API controller for patient-related operations, including dashboard data retrieval, appointment management, and doctor information.

    [ApiController]
   
    [Route("api/patient")]
    public class PatientApiController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly UserRepository _userRepository;

        // Constructor to inject the required services for patient operations and user management.
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

        // Endpoint to retrieve the list of appointments for the authenticated patient. It uses the patient's username to fetch the relevant data from the service layer.

        [HttpGet("appointments")]
        public IActionResult GetAppointments()
        {
            var dashboard = _patientService.GetPatientDashboard(User.Identity?.Name ?? string.Empty);
            return Ok(dashboard.Appointments);
        }

        // Endpoint to retrieve detailed information about a specific appointment by its ID. It checks if the appointment belongs to the authenticated patient or if the user has admin privileges before returning the details.

        [HttpGet("appointments/{id:int}")]
        public IActionResult GetAppointmentDetail(int id)
        {
            var detail = _patientService.GetAppointmentDetail(id, User.Identity?.Name ?? string.Empty, User.IsInRole("Admin"));
            return detail == null ? NotFound() : Ok(detail);
        }


        // Endpoint to book a new appointment. It accepts a request body containing the appointment details and attempts to create the appointment through the service layer. The patient's name is determined based on the user's role, allowing admins to specify a patient name if needed.
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

        // Endpoint to cancel an existing appointment. It checks if the appointment belongs to the authenticated patient before allowing the cancellation. The service layer handles the cancellation logic and returns a success status, which is then communicated back to the client.
        [HttpPost("appointments/{id:int}/cancel")]
        public IActionResult CancelAppointment(int id)
        {
            var patientName = User.Identity?.Name ?? string.Empty;
            var success = _patientService.TryCancelPatientAppointment(id, patientName);
            return success ? Ok(new { message = "Appointment cancelled successfully." }) : BadRequest(new { message = "Unable to cancel the appointment." });
        }

        // Endpoint to reschedule an existing appointment. It accepts a request body containing the new date and time for the appointment. The service layer checks if the appointment belongs to the authenticated patient and attempts to reschedule it, returning a success status that is communicated back to the client.
        [HttpPost("appointments/{id:int}/reschedule")]
        public IActionResult RescheduleAppointment(int id, [FromBody] RescheduleAppointmentRequest request)
        {
            var success = _patientService.ReschedulePatientAppointment(id, User.Identity?.Name ?? string.Empty, request.NewDate, request.NewTime);
            return success ? Ok(new { message = "Appointment rescheduled successfully." }) : BadRequest(new { message = "Unable to reschedule the appointment." });
        }

        // Endpoint to retrieve available time slots for doctors. It accepts an optional query parameter for the doctor's name, allowing clients to filter the slots by a specific doctor if desired. The service layer returns the relevant slot information based on the provided criteria.
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

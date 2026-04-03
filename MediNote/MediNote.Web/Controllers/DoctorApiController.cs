using MediNote.Web.Contracts;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    //BY: Daniel
    // API controller for doctor-related operations, including schedule retrieval, availability management, appointment approval/cancellation, and doctor notes/prescriptions management.
    [ApiController]
  
    [Route("api/doctor")]
    public class DoctorApiController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;
        private readonly AvailabilityService _availabilityService;
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly DoctorPortalService _doctorPortalService;

        public DoctorApiController(
            ScheduleService scheduleService,
            AvailabilityService availabilityService,
            DoctorAppointmentService doctorAppointmentService,
            DoctorPortalService doctorPortalService)
        {
            _scheduleService = scheduleService;
            _availabilityService = availabilityService;
            _doctorAppointmentService = doctorAppointmentService;
            _doctorPortalService = doctorPortalService;
        }

        [HttpGet("schedule")]
        public ActionResult<DoctorScheduleViewModel> GetSchedule()
        {
            var model = _scheduleService.GetDoctorSchedule(User.IsInRole("Admin") ? null : User.Identity?.Name);
            return Ok(model);
        }

        [HttpGet("availability")]
        public ActionResult<ManageAvailabilityViewModel> GetAvailability()
        {
            var model = _availabilityService.GetManageAvailabilityViewModel(User.Identity?.Name, User.IsInRole("Admin"));
            return Ok(model);
        }

        [HttpGet("pendingappointments")]
        public ActionResult<PendingAppointmentsViewModel> GetPendingAppointments()
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel(User.Identity?.Name, User.IsInRole("Admin"));
            return Ok(model);
        }

        [HttpPost("appointments/{id:int}/approve")]
        public IActionResult ApproveAppointment(int id)
        {
            return Ok(new { message = _doctorAppointmentService.ApproveAppointment(id) });
        }

        [HttpPost("appointments/{id:int}/cancel")]
        public IActionResult CancelAppointment(int id)
        {
            return Ok(new { message = _doctorAppointmentService.CancelAppointment(id) });
        }

        [HttpGet("appointments/{id:int}")]
        public IActionResult GetAppointmentDetail(int id)
        {
            var detail = _doctorPortalService.GetAppointmentDetail(id, User.Identity?.Name, User.IsInRole("Admin"));
            return detail == null ? NotFound() : Ok(detail);
        }

        [HttpPost("appointments/{id:int}/notes")]
        public IActionResult AddDoctorNote(int id, [FromBody] DoctorNoteCreateRequest request)
        {
            var ok = _doctorPortalService.AddDoctorNote(id, User.Identity?.Name ?? string.Empty, request.Note, request.FollowUpInstructions, User.IsInRole("Admin"), out var message);
            return ok ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("appointments/{id:int}/prescriptions")]
        public IActionResult AddPrescription(int id, [FromBody] PrescriptionCreateRequest request)
        {
            var ok = _doctorPortalService.AddPrescription(id, User.Identity?.Name ?? string.Empty, request, User.IsInRole("Admin"), out var message);
            return ok ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("appointments/{id:int}/complete")]
        public IActionResult CompleteAppointment(int id)
        {
            var ok = _doctorPortalService.MarkAppointmentCompleted(id, User.Identity?.Name ?? string.Empty, User.IsInRole("Admin"), out var message);
            return ok ? Ok(new { message }) : BadRequest(new { message });
        }

        [HttpPost("appointments/{id:int}/reminders")]
        public IActionResult QueueReminder(int id, [FromBody] QueueReminderRequest request)
        {
            var ok = _doctorPortalService.QueueReminder(id, User.Identity?.Name ?? string.Empty, request.NotificationChannel, request.Recipient, User.IsInRole("Admin"), out var message);
            return ok ? Ok(new { message }) : BadRequest(new { message });
        }
    }
}

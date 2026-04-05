using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.API.Controllers
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// API controller responsible for doctor-related JSON endpoints.
    /// </summary>
    [ApiController]
    [Route("api/doctor")]
    public class DoctorApiController : ControllerBase
    {
        /// <summary>
        /// Stores the schedule service used by the API controller.
        /// </summary>
        private readonly ScheduleService _scheduleService;

        /// <summary>
        /// Stores the availability service used by the API controller.
        /// </summary>
        private readonly AvailabilityService _availabilityService;

        /// <summary>
        /// Stores the doctor appointment service used by the API controller.
        /// </summary>
        private readonly DoctorAppointmentService _doctorAppointmentService;

        /// <summary>
        /// Initializes a new instance of the DoctorApiController class.
        /// </summary>
        /// <param name="scheduleService">The injected schedule service.</param>
        /// <param name="availabilityService">The injected availability service.</param>
        /// <param name="doctorAppointmentService">The injected doctor appointment service.</param>
        public DoctorApiController(
            ScheduleService scheduleService,
            AvailabilityService availabilityService,
            DoctorAppointmentService doctorAppointmentService)
        {
            _scheduleService = scheduleService;
            _availabilityService = availabilityService;
            _doctorAppointmentService = doctorAppointmentService;
        }

        /// <summary>
        /// Returns the doctor's schedule as JSON.
        /// </summary>
        /// <returns>The doctor schedule data.</returns>
        [HttpGet("schedule")]
        public ActionResult<DoctorScheduleViewModel> GetSchedule()
        {
            var model = _scheduleService.GetDoctorSchedule();
            return Ok(model);
        }

        /// <summary>
        /// Returns doctor availability data as JSON.
        /// </summary>
        /// <returns>The doctor availability data.</returns>
        [HttpGet("availability")]
        public ActionResult<ManageAvailabilityViewModel> GetAvailability()
        {
            var model = _availabilityService.GetManageAvailabilityViewModel();
            return Ok(model);
        }

        /// <summary>
        /// Returns pending appointments as JSON.
        /// </summary>
        /// <returns>The pending appointments data.</returns>
        [HttpGet("pendingappointments")]
        public ActionResult<PendingAppointmentsViewModel> GetPendingAppointments()
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            return Ok(model);
        }

        /// <summary>
        /// Returns reschedule appointment details as JSON.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>The reschedule appointment data.</returns>
        [HttpGet("reschedule/{id}")]
        public ActionResult<RescheduleAppointmentViewModel> GetRescheduleDetails(int id)
        {
            var model = _doctorAppointmentService.GetRescheduleViewModel(id);
            return Ok(model);
        }

        /// <summary>
        /// Approves a pending appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>A status message.</returns>
        [HttpPost("approve/{id}")]
        public IActionResult ApproveAppointment(int id)
        {
            var result = _doctorAppointmentService.ApproveAppointment(id);
            if (result.Contains("not found")) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Rejects a pending appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>A status message.</returns>
        [HttpPost("reject/{id}")]
        public IActionResult RejectAppointment(int id)
        {
            var result = _doctorAppointmentService.RejectAppointment(id);
            if (result.Contains("not found")) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Cancels an appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>A status message.</returns>
        [HttpPost("cancel/{id}")]
        public IActionResult CancelAppointment(int id)
        {
            var result = _doctorAppointmentService.CancelAppointment(id);
            if (result.Contains("not found")) return NotFound(result);
            if (result.Contains("cannot be cancelled")) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Reschedules an appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <param name="model">The reschedule details.</param>
        /// <returns>A status message.</returns>
        [HttpPost("reschedule/{id}")]
        public IActionResult RescheduleAppointment(int id, [FromBody] RescheduleAppointmentRequest model)
        {
            if (id != model.AppointmentId)
            {
                return BadRequest("Appointment ID mismatch.");
            }

            var vm = _doctorAppointmentService.GetRescheduleViewModel(id);
            if (vm.PatientName == "Unknown Patient")
            {
                return NotFound($"Appointment #{id} not found.");
            }

            if (!TimeSpan.TryParse(model.NewTime, out TimeSpan timeSpan))
            {
                return BadRequest("Invalid time format. Please use HH:mm.");
            }

            if (!_doctorAppointmentService.TryValidateRescheduleSlot(model.DoctorName, model.NewDate, timeSpan, id, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            var result = _doctorAppointmentService.ConfirmReschedule(id, model.NewDate, timeSpan);
            return Ok(result);
        }

        /// <summary>
        /// Adds doctor availability.
        /// </summary>
        /// <param name="availability">The availability model to add.</param>
        /// <returns>Success message.</returns>
        [HttpPost("availability")]
        public IActionResult AddAvailability([FromBody] MediNote.Web.Models.Availability availability)
        {
            if (!_availabilityService.IsEndTimeAfterStartTime(availability.StartTime, availability.EndTime))
            {
                return BadRequest("End time must be after start time.");
            }

            if (_availabilityService.HasOverlappingSlot(availability.AvailableDate, availability.StartTime, availability.EndTime, availability.DoctorName))
            {
                return BadRequest("This time slot overlaps with an existing availability.");
            }

            _availabilityService.AddAvailability(availability);
            return Ok("Availability added successfully.");
        }
    }

    /// <summary>
    /// Request model for rescheduling an appointment.
    /// </summary>
    public class RescheduleAppointmentRequest
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime NewDate { get; set; }
        public string NewTime { get; set; } = string.Empty;
    }
}
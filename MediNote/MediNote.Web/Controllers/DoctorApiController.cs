using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
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
    }
}
using System.Collections.Generic;
using MediNote.Web.Models;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Controller responsible for doctor-related pages such as schedule,
    /// availability management, pending appointments, and rescheduling.
    /// </summary>
    public class DoctorController : Controller
    {
        /// <summary>
        /// Stores the schedule service used by the controller.
        /// </summary>
        private readonly ScheduleService _scheduleService;

        /// <summary>
        /// Stores the availability service used by the controller.
        /// </summary>
        private readonly AvailabilityService _availabilityService;

        /// <summary>
        /// Stores the appointment service used by the controller.
        /// </summary>
        private readonly DoctorAppointmentService _doctorAppointmentService;

        /// <summary>
        /// Initializes a new instance of the DoctorController class.
        /// </summary>
        /// <param name="scheduleService">The injected schedule service.</param>
        /// <param name="availabilityService">The injected availability service.</param>
        /// <param name="doctorAppointmentService">The injected doctor appointment service.</param>
        public DoctorController(
            ScheduleService scheduleService,
            AvailabilityService availabilityService,
            DoctorAppointmentService doctorAppointmentService)
        {
            _scheduleService = scheduleService;
            _availabilityService = availabilityService;
            _doctorAppointmentService = doctorAppointmentService;
        }

        /// <summary>
        /// Displays the doctor's schedule page.
        /// </summary>
        /// <returns>The Schedule view.</returns>
        public IActionResult Schedule()
        {
            var model = _scheduleService.GetDoctorSchedule();
            return View(model);
        }

        /// <summary>
        /// Displays the page for managing doctor availability.
        /// </summary>
        /// <returns>The ManageAvailability view.</returns>
        [HttpGet]
        public IActionResult ManageAvailability()
        {
            var model = _availabilityService.GetManageAvailabilityViewModel();

            if (model == null)
            {
                model = new ManageAvailabilityViewModel();
            }

            if (model.ExistingSlots == null)
            {
                model.ExistingSlots = new List<Availability>();
            }

            return View(model);
        }

        /// <summary>
        /// Handles submitted availability form data.
        /// </summary>
        /// <param name="model">The submitted availability view model.</param>
        /// <returns>The ManageAvailability view with validation or success message.</returns>
        [HttpPost]
        public IActionResult ManageAvailability(ManageAvailabilityViewModel model)
        {
            if (model == null)
            {
                model = new ManageAvailabilityViewModel();
            }

            model.ExistingSlots = _availabilityService.GetSampleAvailabilitySlots();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_availabilityService.IsEndTimeAfterStartTime(model.StartTime!.Value, model.EndTime!.Value))
            {
                ModelState.AddModelError(string.Empty, "End time must be later than start time.");
                return View(model);
            }

            if (_availabilityService.HasOverlappingSlot(
                model.AvailableDate!.Value,
                model.StartTime!.Value,
                model.EndTime!.Value))
            {
                ModelState.AddModelError(string.Empty, "The selected availability slot overlaps with an existing slot.");
                return View(model);
            }

            var refreshedModel = _availabilityService.GetManageAvailabilityViewModel();

            if (refreshedModel == null)
            {
                refreshedModel = new ManageAvailabilityViewModel();
            }

            if (refreshedModel.ExistingSlots == null)
            {
                refreshedModel.ExistingSlots = new List<Availability>();
            }

            refreshedModel.StatusMessage = "Availability slot saved successfully.";
            return View(refreshedModel);
        }

        /// <summary>
        /// Displays the page showing pending appointments that need doctor action.
        /// </summary>
        /// <returns>The PendingAppointments view.</returns>
        [HttpGet]
        public IActionResult PendingAppointments()
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            return View(model);
        }

        /// <summary>
        /// Handles approval of a pending appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>The PendingAppointments view with a status message.</returns>
        [HttpPost]
        public IActionResult ApprovePendingAppointment(int id)
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            model.StatusMessage = _doctorAppointmentService.ApproveAppointment(id);
            return View("PendingAppointments", model);
        }

        /// <summary>
        /// Handles rejection of a pending appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>The PendingAppointments view with a status message.</returns>
        [HttpPost]
        public IActionResult RejectPendingAppointment(int id)
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            model.StatusMessage = _doctorAppointmentService.RejectAppointment(id);
            return View("PendingAppointments", model);
        }

        /// <summary>
        /// Displays the reschedule page for a selected appointment.
        /// </summary>
        /// <param name="id">The appointment ID.</param>
        /// <returns>The Reschedule view.</returns>
        [HttpGet]
        public IActionResult Reschedule(int id)
        {
            var model = _doctorAppointmentService.GetRescheduleViewModel(id);
            return View(model);
        }

        /// <summary>
        /// Handles submitted reschedule form data.
        /// </summary>
        /// <param name="model">The submitted reschedule appointment view model.</param>
        /// <returns>The Reschedule view with validation or success message.</returns>
        [HttpPost]
        public IActionResult Reschedule(RescheduleAppointmentViewModel model)
        {
            var existingAppointment = _doctorAppointmentService.GetRescheduleViewModel(model.AppointmentId);

            model.PatientName = existingAppointment.PatientName;
            model.CurrentDate = existingAppointment.CurrentDate;
            model.CurrentTime = existingAppointment.CurrentTime;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_doctorAppointmentService.IsAppointmentEligibleForReschedule(model.AppointmentId))
            {
                ModelState.AddModelError(string.Empty, "This appointment cannot be rescheduled.");
                return View(model);
            }

            if (_doctorAppointmentService.HasRescheduleConflict(model.NewDate!.Value, model.NewTime!.Value))
            {
                ModelState.AddModelError(string.Empty, "The selected new time slot is already taken.");
                return View(model);
            }

            var refreshedModel = _doctorAppointmentService.GetRescheduleViewModel(model.AppointmentId);
            refreshedModel.StatusMessage = _doctorAppointmentService.ConfirmReschedule(model.AppointmentId);
            return View(refreshedModel);
        }
    }
}
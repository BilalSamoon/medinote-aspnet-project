using MediNote.Web.Contracts;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    /// <summary>
    /// Controller responsible for doctor-related pages.
    /// </summary>
    [Authorize(Roles = "Doctor,Admin")]
    public class DoctorController : Controller
    {
        private readonly ScheduleService _scheduleService;
        private readonly AvailabilityService _availabilityService;
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly DoctorPortalService _doctorPortalService;

        public DoctorController(
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

        public IActionResult Schedule()
        {
            var doctorName = User.IsInRole("Admin") ? null : User.Identity?.Name;
            var model = _scheduleService.GetDoctorSchedule(doctorName);
            return View(model);
        }

        [HttpGet]
        public IActionResult ManageAvailability()
        {
            var model = _availabilityService.GetManageAvailabilityViewModel(User.Identity?.Name, User.IsInRole("Admin"));
            return View(model);
        }

        [HttpPost]
        public IActionResult ManageAvailability(ManageAvailabilityViewModel model)
        {
            model.ExistingSlots = _availabilityService.GetSampleAvailabilitySlots(User.Identity?.Name, User.IsInRole("Admin"));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_availabilityService.IsEndTimeAfterStartTime(model.StartTime!.Value, model.EndTime!.Value))
            {
                ModelState.AddModelError(string.Empty, "End time must be later than start time.");
                return View(model);
            }

            var doctorName = User.Identity?.Name ?? "Unknown Doctor";
            if (_availabilityService.HasOverlappingSlot(
                model.AvailableDate!.Value,
                model.StartTime!.Value,
                model.EndTime!.Value,
                User.IsInRole("Admin") ? null : doctorName))
            {
                ModelState.AddModelError(string.Empty, "The selected availability slot overlaps with an existing slot for this doctor.");
                return View(model);
            }

            _availabilityService.AddAvailability(new Models.Availability
            {
                DoctorName = doctorName,
                AvailableDate = model.AvailableDate.Value,
                StartTime = model.StartTime.Value,
                EndTime = model.EndTime.Value
            });

            var refreshedModel = _availabilityService.GetManageAvailabilityViewModel(User.Identity?.Name, User.IsInRole("Admin"));
            refreshedModel.StatusMessage = "Availability slot saved successfully.";
            return View(refreshedModel);
        }

        [HttpGet]
        public IActionResult PendingAppointments()
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel(User.Identity?.Name, User.IsInRole("Admin"));
            return View(model);
        }

        [HttpPost]
        public IActionResult ApprovePendingAppointment(int id)
        {
            TempData["DoctorStatusMessage"] = _doctorAppointmentService.ApproveAppointment(id);
            return RedirectToAction(nameof(PendingAppointments));
        }

        [HttpPost]
        public IActionResult CancelPendingAppointment(int id)
        {
            TempData["DoctorStatusMessage"] = _doctorAppointmentService.CancelAppointment(id);
            return RedirectToAction(nameof(PendingAppointments));
        }

        [HttpGet]
        public IActionResult Reschedule(int id)
        {
            var model = _doctorAppointmentService.GetRescheduleViewModel(id);
            return View(model);
        }

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

            var detail = _doctorPortalService.GetAppointmentDetail(model.AppointmentId, User.Identity?.Name, User.IsInRole("Admin"));
            var doctorName = detail?.Appointment.DoctorName ?? User.Identity?.Name ?? string.Empty;
            if (!_doctorAppointmentService.TryValidateRescheduleSlot(doctorName, model.NewDate!.Value, model.NewTime!.Value, model.AppointmentId, out var errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }

            string statusMessage = _doctorAppointmentService.ConfirmReschedule(model.AppointmentId, model.NewDate!.Value, model.NewTime!.Value);
            var refreshedModel = _doctorAppointmentService.GetRescheduleViewModel(model.AppointmentId);
            refreshedModel.StatusMessage = statusMessage;
            return View(refreshedModel);
        }

        [HttpGet]
        public IActionResult AppointmentDetails(int id)
        {
            return View(BuildAppointmentDetailsViewModel(id));
        }

        [HttpPost]
        public IActionResult ApproveAppointment(int appointmentId)
        {
            var message = _doctorAppointmentService.ApproveAppointment(appointmentId);
            return View("AppointmentDetails", BuildAppointmentDetailsViewModel(appointmentId, message));
        }

        [HttpPost]
        public IActionResult AddDoctorNote(DoctorAppointmentDetailsViewModel model)
        {
            _doctorPortalService.AddDoctorNote(model.Detail.Appointment.AppointmentId, User.Identity?.Name ?? string.Empty, model.NewNote, model.FollowUpInstructions, User.IsInRole("Admin"), out var message);
            return View("AppointmentDetails", BuildAppointmentDetailsViewModel(model.Detail.Appointment.AppointmentId, message));
        }

        [HttpPost]
        public IActionResult AddPrescription(DoctorAppointmentDetailsViewModel model)
        {
            _doctorPortalService.AddPrescription(
                model.Detail.Appointment.AppointmentId,
                User.Identity?.Name ?? string.Empty,
                new PrescriptionCreateRequest
                {
                    MedicationName = model.MedicationName,
                    Dosage = model.Dosage,
                    Frequency = model.Frequency,
                    Duration = model.Duration,
                    Instructions = model.PrescriptionInstructions
                },
                User.IsInRole("Admin"),
                out var message);

            return View("AppointmentDetails", BuildAppointmentDetailsViewModel(model.Detail.Appointment.AppointmentId, message));
        }

        [HttpPost]
        public IActionResult MarkCompleted(int appointmentId)
        {
            _doctorPortalService.MarkAppointmentCompleted(appointmentId, User.Identity?.Name ?? string.Empty, User.IsInRole("Admin"), out var message);
            return View("AppointmentDetails", BuildAppointmentDetailsViewModel(appointmentId, message));
        }

        [HttpPost]
        public IActionResult CancelAppointment(int appointmentId)
        {
            _doctorPortalService.CancelAppointment(appointmentId, User.Identity?.Name ?? string.Empty, User.IsInRole("Admin"), out var message);
            return View("AppointmentDetails", BuildAppointmentDetailsViewModel(appointmentId, message));
        }

        [HttpPost]
        public IActionResult QueueReminder(DoctorAppointmentDetailsViewModel model)
        {
            _doctorPortalService.QueueReminder(model.Detail.Appointment.AppointmentId, User.Identity?.Name ?? string.Empty, model.ReminderChannel, model.ReminderRecipient, User.IsInRole("Admin"), out var message);
            return View("AppointmentDetails", BuildAppointmentDetailsViewModel(model.Detail.Appointment.AppointmentId, message));
        }

        private DoctorAppointmentDetailsViewModel BuildAppointmentDetailsViewModel(int appointmentId, string? statusMessage = null)
        {
            var detail = _doctorPortalService.GetAppointmentDetail(appointmentId, User.Identity?.Name, User.IsInRole("Admin"));
            if (detail == null)
            {
                detail = new AppointmentDetailDto();
            }

            return new DoctorAppointmentDetailsViewModel
            {
                Detail = detail,
                StatusMessage = statusMessage ?? string.Empty
            };
        }
    }
}

using System;
using System.Linq;
using MediNote.Web.Contracts;
using MediNote.Web.Data;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly PriorityCalculationService _priorityCalculationService;
        private readonly AdminReportService _adminReportService;
        private readonly MediNoteDbContext _context;
        private readonly PatientService _patientService;
        private readonly UserRepository _userRepository;

        public AdminController(
            DoctorAppointmentService doctorAppointmentService,
            ScheduleService scheduleService,
            PriorityCalculationService priorityCalculationService,
            AdminReportService adminReportService,
            MediNoteDbContext context,
            PatientService patientService,
            AvailabilityService availabilityService,
            UserRepository userRepository)
        {
            _doctorAppointmentService = doctorAppointmentService;
            _priorityCalculationService = priorityCalculationService;
            _adminReportService = adminReportService;
            _context = context;
            _patientService = patientService;
            _userRepository = userRepository;
        }

        public IActionResult Dashboard()
        {
            var recentAppointments = _context.Appointments
                .OrderByDescending(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .Take(6)
                .ToList()
                .Select(a => new AppointmentSummaryDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    RequestedDate = a.RequestedDate,
                    RequestedTime = a.RequestedTime,
                    Symptoms = a.Symptoms,
                    Status = a.Status,
                    CanCancel = a.Status == "Pending" || a.Status == "Approved",
                    HasDoctorNotes = _context.DoctorNotes.Any(n => n.AppointmentId == a.AppointmentId),
                    HasPrescriptions = _context.Prescriptions.Any(p => p.AppointmentId == a.AppointmentId)
                })
                .ToList();

            var model = new AdminDashboardViewModel
            {
                TotalAppointments = _context.Appointments.Count(),
                PendingAppointments = _context.Appointments.Count(a => a.Status == "Pending"),
                ApprovedAppointments = _context.Appointments.Count(a => a.Status == "Approved"),
                CancelledAppointments = _context.Appointments.Count(a => a.Status == "Cancelled"),
                CompletedAppointments = _context.Appointments.Count(a => a.Status == "Completed"),
                RecentAppointments = recentAppointments
            };

            return View(model);
        }

        public IActionResult Reports()
        {
            ViewBag.TotalAppointments = _adminReportService.GetTotalAppointments(_context.Appointments.Count());
            ViewBag.PendingAppointments = _adminReportService.GetPendingAppointments(_context.Appointments.Count(a => a.Status == "Pending"));
            return View();
        }

        public IActionResult PriorityReport()
        {
            var pendingAppointmentsModel = _doctorAppointmentService.GetPendingAppointmentsViewModel();

            var priorityItems = pendingAppointmentsModel.PendingAppointments
                .Select(appointment => new PriorityReportItemViewModel
                {
                    AppointmentId = appointment.AppointmentId,
                    PatientName = appointment.PatientName,
                    Symptoms = appointment.Symptoms,
                    Priority = _priorityCalculationService.GetPriority(appointment.Symptoms)
                })
                .ToList();

            return View(priorityItems);
        }

        public IActionResult ManageAppointments(string? status = null)
        {
            var appointments = _doctorAppointmentService.GetAppointmentsForManagement(null, true);
            if (!string.IsNullOrWhiteSpace(status))
            {
                appointments = appointments.Where(a => string.Equals(a.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var model = new AdminManageAppointmentsViewModel
            {
                SelectedStatusFilter = status ?? string.Empty,
                StatusMessage = TempData["AdminStatusMessage"]?.ToString() ?? string.Empty,
                Doctors = _userRepository.GetDoctors().Select(d => d.DisplayName).ToList(),
                DoctorSlots = _patientService.GetDoctorSlots(),
                BookableSlots = _patientService.GetBookableSlotOptions(),
                Appointments = appointments.Select(a => new AdminAppointmentItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    RequestedDate = a.RequestedDate,
                    RequestedTime = a.RequestedTime,
                    Symptoms = a.Symptoms,
                    Status = a.Status,
                    ContactRecipient = a.ContactRecipient
                }).ToList(),
                NewAppointment = new AdminAppointmentCreateViewModel
                {
                    RequestedDate = DateTime.Today.AddDays(1),
                    NotificationChannel = "Email"
                }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ApproveAppointment(int id)
        {
            TempData["AdminStatusMessage"] = _doctorAppointmentService.ApproveAppointment(id);
            return RedirectToAction(nameof(ManageAppointments));
        }

        [HttpPost]
        public IActionResult CancelAppointment(int id)
        {
            TempData["AdminStatusMessage"] = _doctorAppointmentService.CancelAppointment(id);
            return RedirectToAction(nameof(ManageAppointments));
        }

        [HttpPost]
        public IActionResult CreateAppointmentOnBehalf(AdminManageAppointmentsViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewAppointment.PatientName) || string.IsNullOrWhiteSpace(model.NewAppointment.DoctorName) || string.IsNullOrWhiteSpace(model.NewAppointment.Symptoms))
            {
                TempData["AdminStatusMessage"] = "Patient name, doctor, and symptoms are required.";
                return RedirectToAction(nameof(ManageAppointments));
            }

            var ok = _patientService.TryBookNewAppointment(
                model.NewAppointment.PatientName,
                model.NewAppointment.DoctorName,
                model.NewAppointment.RequestedDate,
                model.NewAppointment.RequestedTime,
                model.NewAppointment.Symptoms,
                model.NewAppointment.ContactRecipient,
                model.NewAppointment.NotificationChannel,
                out _,
                out var errorMessage);

            TempData["AdminStatusMessage"] = ok
                ? "Appointment booked successfully on behalf of the patient."
                : errorMessage;

            return RedirectToAction(nameof(ManageAppointments));
        }
    }
}

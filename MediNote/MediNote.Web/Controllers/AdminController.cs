using System.Linq;
using   MediNote.Web.Data;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MediNote.Web.Controllers
{
    /// Author: Bilal Ahmed Samoon
    /// Controller responsible for admin-related pages such as dashboard, reports, and appointment priority review.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly ScheduleService _scheduleService;
        private readonly PriorityCalculationService _priorityCalculationService;
        private readonly AdminReportService _adminReportService;
        private readonly MediNoteDbContext _context;

        public AdminController(
            DoctorAppointmentService doctorAppointmentService,
            ScheduleService scheduleService,
            PriorityCalculationService priorityCalculationService,
            AdminReportService adminReportService,
            MediNoteDbContext context)
        {
            _doctorAppointmentService = doctorAppointmentService;
            _scheduleService = scheduleService;
            _priorityCalculationService = priorityCalculationService;
            _adminReportService = adminReportService;
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Reports()
        {
            var pendingAppointmentsModel = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            var doctorScheduleModel = _scheduleService.GetDoctorSchedule();

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

        public IActionResult ManageAppointments()
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult ApproveAppointment(int id)
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            model.StatusMessage = _doctorAppointmentService.ApproveAppointment(id);

            return View("ManageAppointments", model);
        }

        [HttpPost]
        public IActionResult RejectAppointment(int id)
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            model.StatusMessage = _doctorAppointmentService.RejectAppointment(id);

            return View("ManageAppointments", model);
        }
    }
}
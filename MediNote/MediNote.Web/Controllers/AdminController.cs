using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;
using MediNote.Web.Models;

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

        // DASHBOARD → API
        public async Task<IActionResult> Dashboard()
        {
            using var client = new HttpClient();

            var url = "https://localhost:7023/api/admin/stats";

            var stats = await client.GetFromJsonAsync<AdminStatsDto>(url);

            ViewBag.TotalAppointments = stats?.Total ?? 0;
            ViewBag.PendingAppointments = stats?.Pending ?? 0;

            return View();
        }

        //REPORTS → API
        public async Task<IActionResult> Reports(string status)
        {
            using var client = new HttpClient();

            var url = "https://localhost:7023/api/admin/all";

            var data = await client.GetFromJsonAsync<List<Appointment>>(url)
                       ?? new List<Appointment>();

            if (!string.IsNullOrEmpty(status))
            {
                data = data.Where(a => a.Status == status).ToList();
            }

            return View(data);
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

    // DTO for API stats
    public class AdminStatsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
    }
}
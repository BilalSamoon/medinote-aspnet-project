using System.Linq;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    /// Author: Bilal Ahmed Samoon
    /// Controller responsible for admin-related pages such as dashboard, reports, and appointment priority review.
    public class AdminController : Controller
    {
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly ScheduleService _scheduleService;
        private readonly PriorityCalculationService _priorityCalculationService;

        public AdminController(
            DoctorAppointmentService doctorAppointmentService,
            ScheduleService scheduleService,
            PriorityCalculationService priorityCalculationService)
        {
            _doctorAppointmentService = doctorAppointmentService;
            _scheduleService = scheduleService;
            _priorityCalculationService = priorityCalculationService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Reports()
        {
            var pendingAppointmentsModel = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            var doctorScheduleModel = _scheduleService.GetDoctorSchedule();

            ViewBag.TotalAppointments = doctorScheduleModel.Appointments.Count;
            ViewBag.PendingAppointments = pendingAppointmentsModel.PendingAppointments.Count;

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

        //Admin manages appointments
        public IActionResult ManageAppointments()
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            return View(model);
        }

        //Approve
        [HttpPost]
        public IActionResult ApproveAppointment(int id)
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            model.StatusMessage = _doctorAppointmentService.ApproveAppointment(id);

            return View("ManageAppointments", model);
        }

        //Reject
        [HttpPost]
        public IActionResult RejectAppointment(int id)
        {
            var model = _doctorAppointmentService.GetPendingAppointmentsViewModel();
            model.StatusMessage = _doctorAppointmentService.RejectAppointment(id);

            return View("ManageAppointments", model);
        }
    }
}
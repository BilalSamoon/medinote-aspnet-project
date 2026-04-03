using MediNote.Web.Contracts;
using MediNote.Web.Models;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http.Json;

namespace MediNote.Web.Controllers
{
    /// <summary>
    /// Author: Bilal Ahmed Samoon
    /// Controller responsible for admin-related pages such as dashboard, reports, appointment management, and user account generation.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly PriorityCalculationService _priorityCalculationService;
        private readonly PatientService _patientService;
        private readonly UserRepository _userRepository;

        public AdminController(
            DoctorAppointmentService doctorAppointmentService,
            PriorityCalculationService priorityCalculationService,
            PatientService patientService,
            UserRepository userRepository)
        {
            _doctorAppointmentService = doctorAppointmentService;
            _priorityCalculationService = priorityCalculationService;
            _patientService = patientService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/dashboard";

            var dto = await client.GetFromJsonAsync<AdminDashboardSummaryDto>(url)
                      ?? new AdminDashboardSummaryDto();

            var model = new AdminDashboardViewModel
            {
                TotalAppointments = dto.TotalAppointments,
                PendingAppointments = dto.PendingAppointments,
                ApprovedAppointments = dto.ApprovedAppointments,
                CancelledAppointments = dto.CancelledAppointments,
                CompletedAppointments = dto.CompletedAppointments,
                RecentAppointments = dto.RecentAppointments ?? new List<AppointmentSummaryDto>()
            };

            return View(model);
        }

        public async Task<IActionResult> Reports(string? status = null)
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/appointments";

            if (!string.IsNullOrWhiteSpace(status))
            {
                url += $"?status={status}";
            }

            var data = await client.GetFromJsonAsync<List<Appointment>>(url)
                       ?? new List<Appointment>();

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

        public async Task<IActionResult> ManageAppointments(string? status = null)
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/appointments";

            if (!string.IsNullOrWhiteSpace(status))
            {
                url += $"?status={status}";
            }

            var appointments = await client.GetFromJsonAsync<List<Appointment>>(url)
                               ?? new List<Appointment>();

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
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/appointments/{id}/approve";

            await client.PostAsync(url, null);

            TempData["AdminStatusMessage"] = "Appointment approved successfully.";
            return RedirectToAction(nameof(ManageAppointments));
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/appointments/{id}/cancel";

            await client.PostAsync(url, null);

            TempData["AdminStatusMessage"] = "Appointment cancelled successfully.";
            return RedirectToAction(nameof(ManageAppointments));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentOnBehalf(AdminManageAppointmentsViewModel model)
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/appointments";

            var request = new BookAppointmentRequest
            {
                PatientName = model.NewAppointment.PatientName,
                DoctorName = model.NewAppointment.DoctorName,
                RequestedDate = model.NewAppointment.RequestedDate,
                RequestedTime = model.NewAppointment.RequestedTime,
                Symptoms = model.NewAppointment.Symptoms,
                ContactRecipient = model.NewAppointment.ContactRecipient,
                NotificationChannel = model.NewAppointment.NotificationChannel
            };

            var response = await client.PostAsJsonAsync(url, request);

            TempData["AdminStatusMessage"] = response.IsSuccessStatusCode
                ? "Appointment booked successfully."
                : "Failed to create appointment.";

            return RedirectToAction(nameof(ManageAppointments));
        }

        public IActionResult GenerateUser()
        {
            return View(new AdminGenerateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> GenerateUser(AdminGenerateUserViewModel model)
        {
            using var client = new HttpClient();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/api/admin/users/generate";

            var response = await client.PostAsJsonAsync(url, model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "User creation failed.");
                return View(model);
            }

            model.SuccessMessage = "User created successfully.";
            ModelState.Clear();

            return View(model);
        }
    }
}
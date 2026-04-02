using MediNote.Web.Contracts;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace MediNote.Web.Pages.Patient
{
    [Authorize(Roles = "Patient,Admin")]
    public class BookAppointmentModel : PageModel
    {
        private readonly PatientService _patientService;
        private readonly UserRepository _userRepository;

        public BookAppointmentModel(PatientService patientService, UserRepository userRepository)
        {
            _patientService = patientService;
            _userRepository = userRepository;
        }

        [BindProperty]
        public PatientAppointmentRequestViewModel RequestModel { get; set; } = new PatientAppointmentRequestViewModel();

        public List<SelectListItem> AvailableDoctors { get; set; } = new();
        public List<DoctorSlotDto> AvailableDoctorSlots { get; set; } = new();
        public List<BookableSlotDto> BookableSlots { get; set; } = new();

        public void OnGet()
        {
            PopulateDoctors();
            PopulateBookingData();
        }

        public IActionResult OnPost()
        {
            PopulateDoctors();
            PopulateBookingData();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string patientName = User.Identity?.Name ?? "Unknown";
            var ok = _patientService.TryBookNewAppointment(
                patientName,
                RequestModel.DoctorName,
                RequestModel.RequestedDate,
                RequestModel.RequestedTime,
                RequestModel.Symptoms,
                RequestModel.ContactRecipient,
                RequestModel.NotificationChannel,
                out _,
                out var errorMessage);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }

            return RedirectToPage("/Patient/Dashboard", new { booked = true });
        }

        private void PopulateDoctors()
        {
            var doctors = _userRepository.GetDoctors().Select(u => u.DisplayName).ToList();
            AvailableDoctors = doctors.Select(d => new SelectListItem { Value = d, Text = d }).ToList();
        }

        private void PopulateBookingData()
        {
            AvailableDoctorSlots = _patientService.GetDoctorSlots().Take(16).ToList();
            BookableSlots = _patientService.GetBookableSlotOptions();

            if (string.IsNullOrWhiteSpace(RequestModel.DoctorName) && AvailableDoctors.Any())
            {
                RequestModel.DoctorName = AvailableDoctors.First().Value ?? string.Empty;
            }
        }
    }
}

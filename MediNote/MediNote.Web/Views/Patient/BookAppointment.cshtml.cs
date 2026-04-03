using MediNote.Web.Contracts;
using MediNote.Web.Services;
using MediNote.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;

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
        public PatientAppointmentRequestViewModel RequestModel { get; set; } = new();

        public List<SelectListItem> AvailableDoctors { get; set; } = new();
        public List<DoctorSlotDto> AvailableDoctorSlots { get; set; } = new();
        public List<BookableSlotDto> BookableSlots { get; set; } = new();

        public void OnGet()
        {
            PopulateDoctors();
            PopulateBookingData();
        }

        public async Task<IActionResult> OnPost()
        {
            PopulateDoctors();
            PopulateBookingData();

            if (!ModelState.IsValid)
                return Page();

            using var client = new HttpClient();

            var url = $"{Request.Scheme}://{Request.Host}/api/patient/appointments";

            var request = new BookAppointmentRequest
            {
                DoctorName = RequestModel.DoctorName,
                RequestedDate = RequestModel.RequestedDate,
                RequestedTime = RequestModel.RequestedTime,
                Symptoms = RequestModel.Symptoms,
                ContactRecipient = RequestModel.ContactRecipient,
                NotificationChannel = RequestModel.NotificationChannel
            };

            var response = await client.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            return RedirectToPage("/Patient/Dashboard", new { booked = true });
        }

        private void PopulateDoctors()
        {
            var doctors = _userRepository.GetDoctors()
                .Select(u => u.DisplayName)
                .ToList();

            AvailableDoctors = doctors
                .Select(d => new SelectListItem { Value = d, Text = d })
                .ToList();
        }

        private void PopulateBookingData()
        {
            AvailableDoctorSlots = _patientService.GetDoctorSlots().Take(16).ToList();
            BookableSlots = _patientService.GetBookableSlotOptions();
        }
    }
}
using MediNote.Web.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediNote.Web.Pages.Patient
{
    [Authorize(Roles = "Patient,Admin")]
    public class BookAppointmentModel : PageModel
    {
        private readonly PatientService _patientService;

        public BookAppointmentModel(PatientService patientService)
        {
            _patientService = patientService;
        }

        [BindProperty]
        public PatientAppointmentRequestViewModel RequestModel { get; set; } = new PatientAppointmentRequestViewModel();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string patientName = User.Identity?.Name ?? "Unknown";

            // Add the appt
            _patientService.BookNewAppointment(
                patientName, 
                RequestModel.DoctorName, 
                RequestModel.RequestedDate, 
                RequestModel.RequestedTime, 
                RequestModel.Symptoms
            );

            return RedirectToPage("/Patient/Dashboard");
        }
    }
}
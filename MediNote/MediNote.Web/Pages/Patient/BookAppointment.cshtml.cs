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
        private readonly AppointmentRepository _appointmentRepository;

        public BookAppointmentModel(AppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        [BindProperty, Required]
        public string DoctorName { get; set; } = string.Empty;

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime RequestedDate { get; set; } = DateTime.Today.AddDays(1);

        [BindProperty, Required]
        public string RequestedTime { get; set; } = "10:00";

        [BindProperty, Required(ErrorMessage = "Please provide your symptoms so the doctor can prepare.")]
        public string Symptoms { get; set; } = string.Empty;

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
            _appointmentRepository.BookAppointment(
                patientName, 
                DoctorName, 
                RequestedDate, 
                RequestedTime, 
                Symptoms
            );

            return RedirectToPage("/Patient/Dashboard");
        }
    }
}
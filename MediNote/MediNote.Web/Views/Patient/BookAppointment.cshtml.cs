using MediNote.Web.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediNote.Web.Pages.Patient
{
    [Authorize(Roles = "Patient,Admin")]
    public class BookAppointmentModel : PageModel
    {
        private readonly PatientService _patientService;
        private readonly MediNoteDbContext _context;

        public BookAppointmentModel(PatientService patientService, MediNoteDbContext context)
        {
            _patientService = patientService;
            _context = context;
        }

        [BindProperty]
        public PatientAppointmentRequestViewModel RequestModel { get; set; } = new PatientAppointmentRequestViewModel();

        public List<SelectListItem> AvailableDoctors { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            PopulateDoctors();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                PopulateDoctors();
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

        private void PopulateDoctors()
        {
            var doctors = _context.Users.Where(u => u.Role == "Doctor").Select(u => u.Username).ToList();
            if(!doctors.Any())
            {
                doctors.Add("Dr. Smith"); // fallback
            }

            AvailableDoctors = doctors.Select(d => new SelectListItem { Value = d, Text = d }).ToList();
        }
    }
}
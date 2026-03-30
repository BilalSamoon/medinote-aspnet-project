using MediNote.Web.Models;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace MediNote.Web.Pages.Patient
{
    [Authorize(Roles = "Patient,Admin")]
    public class DashboardModel : PageModel
    {
        private readonly PatientService _patientService;

        public DashboardModel(PatientService patientService)
        {
            _patientService = patientService;
        }

        public IList<Appointment> Appointments { get; set; } = new List<Appointment>();

        public void OnGet()
        {
            string patientName = User.Identity?.Name ?? string.Empty;
            Appointments = _patientService.GetPatientAppointments(patientName);
        }

        public IActionResult OnPostCancel(int id)
        {
            string patientName = User.Identity?.Name ?? string.Empty;
            _patientService.CancelPatientAppointment(id, patientName);

            return RedirectToPage();
        }
    }
}
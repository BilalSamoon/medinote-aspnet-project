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
        private readonly AppointmentRepository _appointmentRepository;

        public DashboardModel(AppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public IList<Appointment> Appointments { get; set; } = new List<Appointment>();

        public void OnGet()
        {
            string patientName = User.Identity?.Name ?? string.Empty;
            Appointments = _appointmentRepository.GetAppointmentsByPatient(patientName);
        }

        public IActionResult OnPostCancel(int id)
        {
            string patientName = User.Identity?.Name ?? string.Empty;
            _appointmentRepository.CancelAppointment(id, patientName);
            
            return RedirectToPage();
        }
    }
}
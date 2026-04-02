using MediNote.Web.Contracts;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public PatientDashboardDto Dashboard { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            string patientName = User.Identity?.Name ?? string.Empty;
            Dashboard = _patientService.GetPatientDashboard(patientName);
        }

        public IActionResult OnPostCancel(int id)
        {
            string patientName = User.Identity?.Name ?? string.Empty;
            var success = _patientService.TryCancelPatientAppointment(id, patientName);
            StatusMessage = success ? "Appointment cancelled successfully." : "Unable to cancel the appointment.";
            return RedirectToPage();
        }
    }
}

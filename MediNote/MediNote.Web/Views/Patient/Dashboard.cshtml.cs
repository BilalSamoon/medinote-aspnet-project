using MediNote.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace MediNote.Web.Pages.Patient
{
    [Authorize(Roles = "Patient,Admin")]
    public class DashboardModel : PageModel
    {
        public PatientDashboardDto Dashboard { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGet()
        {
            using var client = new HttpClient();

            var url = $"{Request.Scheme}://{Request.Host}/api/patient/dashboard";

            var data = await client.GetFromJsonAsync<PatientDashboardDto>(url);

            Dashboard = data ?? new PatientDashboardDto();
        }

        public async Task<IActionResult> OnPostCancel(int id)
        {
            using var client = new HttpClient();

            var url = $"{Request.Scheme}://{Request.Host}/api/patient/appointments/{id}/cancel";

            var response = await client.PostAsync(url, null);

            StatusMessage = response.IsSuccessStatusCode
                ? "Appointment cancelled successfully."
                : "Unable to cancel the appointment.";

            return RedirectToPage();
        }
    }
}
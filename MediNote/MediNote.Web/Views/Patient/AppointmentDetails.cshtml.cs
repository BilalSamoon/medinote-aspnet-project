using MediNote.Web.Contracts;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediNote.Web.Pages.Patient
{
    [Authorize(Roles = "Patient,Admin")]
    public class AppointmentDetailsModel : PageModel
    {
        private readonly PatientService _patientService;

        public AppointmentDetailsModel(PatientService patientService)
        {
            _patientService = patientService;
        }

        public AppointmentDetailDto Detail { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var detail = _patientService.GetAppointmentDetail(id, User.Identity?.Name ?? string.Empty, User.IsInRole("Admin"));
            if (detail == null)
            {
                return RedirectToPage("/Patient/Dashboard");
            }

            Detail = detail;
            return Page();
        }
    }
}

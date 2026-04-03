using System.Collections.Generic;
using MediNote.Web.Contracts;

namespace MediNote.Web.ViewModels
{
    // By: Camila Esguerra
    // ViewModel for the admin dashboard, providing aggregated appointment data and recent activity summaries.
    public class AdminDashboardViewModel
    {
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public List<AppointmentSummaryDto> RecentAppointments { get; set; } = new();
    }
}

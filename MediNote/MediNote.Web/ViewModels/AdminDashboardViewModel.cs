using System.Collections.Generic;
using MediNote.Web.Contracts;

namespace MediNote.Web.ViewModels
{
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

namespace MediNote.Web.ViewModels
{
    /// Author: Bilal Ahmed Samoon
    /// View model used by the admin priority report page.
    public class PriorityReportItemViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
    }
}
using MediNote.Web.Contracts;

namespace MediNote.Web.ViewModels
{
    /// By: Daniel
    // ViewModel for doctor appointment details page, including appointment info, doctor notes, prescriptions, and reminder setup.
    public class DoctorAppointmentDetailsViewModel
    {
        public AppointmentDetailDto Detail { get; set; } = new();
        public string StatusMessage { get; set; } = string.Empty;
        public string NewNote { get; set; } = string.Empty;
        public string FollowUpInstructions { get; set; } = string.Empty;
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string PrescriptionInstructions { get; set; } = string.Empty;
        public string ReminderChannel { get; set; } = "Email";
        public string ReminderRecipient { get; set; } = string.Empty;
    }
}

namespace MediNote.Web.Models
{
    /// Author: Bilal Ahmed Samoon
    /// Represents an appointment entity for database persistence.
    public class Appointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
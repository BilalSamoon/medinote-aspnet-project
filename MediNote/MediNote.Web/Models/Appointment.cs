using System;

namespace MediNote.Web.Models
{

    //By: Camila Esguerra
    // Represents an appointment request made by a patient to see a doctor. Contains details about the patient, doctor, requested date and time, symptoms, and the status of the appointment.
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string RequestedTime { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled
    }
}

using System;
using System.ComponentModel.DataAnnotations;


namespace MediNote.Web.ViewModels
{
    //By: Camila Esguerra
    /// ViewModel for patients to request new appointments, including doctor selection, date/time, and symptom description.
    public class PatientAppointmentRequestViewModel
    {
        [Required]
        [Display(Name = "Doctor Name")]
        public string DoctorName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Display(Name = "Requested Time")]
        public string RequestedTime { get; set; } = "10:00";

        [Required(ErrorMessage = "Please provide your symptoms so the doctor can prepare.")]
        [Display(Name = "Symptoms/Reason for Visit")]
        public string Symptoms { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using MediNote.Web.Contracts;

namespace MediNote.Web.ViewModels
{
    // By: Camila Esguerra
    // ViewModel for the admin appointment management page, encapsulating all necessary data for displaying and managing appointments.
    public class AdminManageAppointmentsViewModel
    {
        public string StatusMessage { get; set; } = string.Empty;
        public string SelectedStatusFilter { get; set; } = string.Empty;
        public List<AdminAppointmentItemViewModel> Appointments { get; set; } = new();
        public List<string> Doctors { get; set; } = new();
        public List<DoctorSlotDto> DoctorSlots { get; set; } = new();
        public List<BookableSlotDto> BookableSlots { get; set; } = new();
        public AdminAppointmentCreateViewModel NewAppointment { get; set; } = new();
    }

    public class AdminAppointmentItemViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string RequestedTime { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ContactRecipient { get; set; } = string.Empty;
    }

    public class AdminAppointmentCreateViewModel
    {
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; } = DateTime.Today.AddDays(1);
        public string RequestedTime { get; set; } = "09:00";
        public string Symptoms { get; set; } = string.Empty;
        public string ContactRecipient { get; set; } = string.Empty;
        public string NotificationChannel { get; set; } = "Email";
    }
}

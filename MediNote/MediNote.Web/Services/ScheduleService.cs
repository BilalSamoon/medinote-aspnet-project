using System;
using MediNote.Web.ViewModels;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Provides schedule-related data for doctor pages.
    /// </summary>
    public class ScheduleService
    {
        /// <summary>
        /// Returns sample schedule data for the doctor schedule page.
        /// </summary>
        /// <returns>A populated doctor schedule view model.</returns>
        public DoctorScheduleViewModel GetDoctorSchedule()
        {
            return new DoctorScheduleViewModel
            {
                DoctorName = "Dr. Daniel Guillaumont",
                ScheduleDate = new DateTime(2026, 3, 29),
                Appointments =
                {
                    new DoctorScheduleItemViewModel
                    {
                        PatientName = "Victoria Zhou",
                        AppointmentDate = new DateTime(2026, 3, 29),
                        AppointmentTime = "9:00 AM",
                        Status = "Approved"
                    },
                    new DoctorScheduleItemViewModel
                    {
                        PatientName = "Aidan Kumar",
                        AppointmentDate = new DateTime(2026, 3, 29),
                        AppointmentTime = "11:30 AM",
                        Status = "Pending"
                    },
                    new DoctorScheduleItemViewModel
                    {
                        PatientName = "Jamal Ishani",
                        AppointmentDate = new DateTime(2026, 3, 29),
                        AppointmentTime = "2:00 PM",
                        Status = "Rescheduled"
                    }
                }
            };
        }
    }
}
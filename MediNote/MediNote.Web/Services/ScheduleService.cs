using System;
using MediNote.Web.ViewModels;
using MediNote.Web.Models;
using System.Linq;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Provides schedule-related data for doctor pages.
    /// </summary>
    public class ScheduleService
    {
        private readonly AppointmentRepository _appointmentRepository;

        public ScheduleService(AppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        /// <summary>
        /// Returns sample schedule data for the doctor schedule page.
        /// </summary>
        /// <returns>A populated doctor schedule view model.</returns>
        public DoctorScheduleViewModel GetDoctorSchedule(string doctorName = null)
        {
            var appointments = _appointmentRepository.GetAllAppointments();
            if (!string.IsNullOrEmpty(doctorName))
            {
                appointments = appointments.Where(a => a.DoctorName == doctorName).ToList();
            }

            return new DoctorScheduleViewModel
            {
                DoctorName = doctorName ?? "Dr. Daniel Guillaumont",
                ScheduleDate = DateTime.Now.Date,
                Appointments = appointments.Select(a => new DoctorScheduleItemViewModel
                {
                    PatientName = a.PatientName,
                    AppointmentDate = a.RequestedDate,
                    AppointmentTime = a.RequestedTime,
                    Status = a.Status
                }).ToList()
            };
        }
    }
}
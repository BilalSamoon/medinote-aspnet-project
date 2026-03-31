using System;
using System.Linq;
using MediNote.Web.ViewModels;
using MediNote.Web.Data;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Provides schedule-related data for doctor pages.
    /// </summary>
    public class ScheduleService
    {
        private readonly MediNoteDbContext _context;

        public ScheduleService(MediNoteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns schedule data dynamically from the database.
        /// </summary>
        /// <returns>A populated doctor schedule view model.</returns>
        public DoctorScheduleViewModel GetDoctorSchedule(string doctorName = null)
        {
            var query = _context.Appointments.AsQueryable();
            if (!string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            var appointments = query.ToList();

            return new DoctorScheduleViewModel
            {
                DoctorName = string.IsNullOrEmpty(doctorName) ? "All Doctors" : doctorName,
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
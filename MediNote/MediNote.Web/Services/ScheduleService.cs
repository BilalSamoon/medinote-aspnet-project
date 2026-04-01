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
        private readonly PriorityCalculationService _priorityCalculationService;

        public ScheduleService(MediNoteDbContext context, PriorityCalculationService priorityCalculationService)
        {
            _context = context;
            _priorityCalculationService = priorityCalculationService;
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
                    DoctorName = a.DoctorName,
                    AppointmentDate = a.RequestedDate,
                    AppointmentTime = a.RequestedTime,
                    Priority = _priorityCalculationService.GetPriority(a.Symptoms),
                    Status = a.Status
                }).ToList()
            };
        }
    }
}
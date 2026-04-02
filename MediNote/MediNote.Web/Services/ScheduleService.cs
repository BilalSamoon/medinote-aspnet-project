using System;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.ViewModels;

namespace MediNote.Web.Services
{
    /// <summary>
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

        public DoctorScheduleViewModel GetDoctorSchedule(string? doctorName = null)
        {
            var query = _context.Appointments.AsQueryable();
            if (!string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(a => a.DoctorName == doctorName);
            }

            var appointments = query
                .OrderBy(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();

            return new DoctorScheduleViewModel
            {
                DoctorName = string.IsNullOrEmpty(doctorName) ? "All Doctors" : doctorName,
                ScheduleDate = DateTime.Now.Date,
                Appointments = appointments.Select(a => new DoctorScheduleItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    AppointmentDate = a.RequestedDate,
                    AppointmentTime = a.RequestedTime,
                    Priority = _priorityCalculationService.GetPriority(a.Symptoms),
                    Status = a.Status,
                    Symptoms = a.Symptoms
                }).ToList()
            };
        }
    }
}

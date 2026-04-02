using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Repository responsible for managing appointment data.
    /// </summary>
    public class AppointmentRepository
    {
        private readonly MediNoteDbContext _context;

        public AppointmentRepository(MediNoteDbContext context)
        {
            _context = context;
        }

        public List<Appointment> GetAllAppointments()
            => _context.Appointments
                .OrderByDescending(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();

        public List<Appointment> GetAppointmentsByPatient(string patientName)
            => _context.Appointments
                .Where(a => a.PatientName == patientName)
                .OrderByDescending(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();

        public List<Appointment> GetPendingAppointments()
            => _context.Appointments
                .Where(a => a.Status == "Pending")
                .OrderBy(a => a.RequestedDate)
                .ThenBy(a => a.RequestedTime)
                .ToList();

        public Appointment? GetAppointmentById(int id)
            => _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);

        public Appointment BookAppointment(string patientName, string doctorName, DateTime date, string time, string symptoms, string? contactRecipient = null, string notificationChannel = "InApp")
        {
            var appointment = new Appointment
            {
                PatientName = patientName,
                DoctorName = doctorName,
                RequestedDate = date,
                RequestedTime = time,
                Symptoms = symptoms,
                Status = "Pending",
                ContactRecipient = contactRecipient ?? string.Empty,
                NotificationChannel = string.IsNullOrWhiteSpace(notificationChannel) ? "InApp" : notificationChannel,
                CreatedAtUtc = DateTime.UtcNow,
                LastUpdatedAtUtc = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return appointment;
        }

        public bool CancelAppointment(int id, string patientName)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id && a.PatientName == patientName);
            if (appointment != null && (appointment.Status == "Pending" || appointment.Status == "Approved"))
            {
                appointment.Status = "Cancelled";
                appointment.LastUpdatedAtUtc = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateStatus(int id, string status)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment != null)
            {
                appointment.Status = status;
                appointment.LastUpdatedAtUtc = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RescheduleAppointment(int id, DateTime newDate, string newTime)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment != null)
            {
                appointment.RequestedDate = newDate;
                appointment.RequestedTime = newTime;
                appointment.LastUpdatedAtUtc = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

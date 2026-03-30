using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    public class AppointmentRepository
    {
        private readonly List<Appointment> _appointments;
        private int _nextId = 3;

        public AppointmentRepository()
        {
            _appointments = new List<Appointment>
            {
                new Appointment
                {
                    AppointmentId = 1,
                    PatientName = "patient1",
                    DoctorName = "Dr. Smith",
                    RequestedDate = new DateTime(2026, 3, 30),
                    RequestedTime = "10:00 AM",
                    Symptoms = "Headache and fever",
                    Status = "Pending"
                },
                new Appointment
                {
                    AppointmentId = 2,
                    PatientName = "patient2",
                    DoctorName = "Dr. Adams",
                    RequestedDate = new DateTime(2026, 3, 31),
                    RequestedTime = "11:00 AM",
                    Symptoms = "Chest discomfort",
                    Status = "Approved"
                }
            };
        }

        public List<Appointment> GetAllAppointments() => _appointments;

        public List<Appointment> GetAppointmentsByPatient(string patientName)
            => _appointments.Where(a => a.PatientName == patientName).ToList();

        public List<Appointment> GetPendingAppointments()
            => _appointments.Where(a => a.Status == "Pending").ToList();

        public Appointment GetAppointmentById(int id)
            => _appointments.FirstOrDefault(a => a.AppointmentId == id);

        public Appointment BookAppointment(string patientName, string doctorName, DateTime date, string time, string symptoms)
        {
            var appointment = new Appointment
            {
                AppointmentId = _nextId++,
                PatientName = patientName,
                DoctorName = doctorName,
                RequestedDate = date,
                RequestedTime = time,
                Symptoms = symptoms,
                Status = "Pending"
            };
            _appointments.Add(appointment);
            return appointment;
        }

        public bool CancelAppointment(int id, string patientName)
        {
            var appointment = _appointments.FirstOrDefault(a => a.AppointmentId == id && a.PatientName == patientName);
            if (appointment != null && appointment.Status != "Cancelled")
            {
                appointment.Status = "Cancelled";
                return true;
            }
            return false;
        }

        public bool UpdateStatus(int id, string status)
        {
            var appointment = _appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment != null)
            {
                appointment.Status = status;
                return true;
            }
            return false;
        }
    }
}

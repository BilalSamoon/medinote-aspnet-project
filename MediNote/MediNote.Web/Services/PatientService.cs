using System;
using System.Collections.Generic;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Camila Esguerra
    /// Patient service for handling patient-specific operations like viewing and managing appointments.
    /// </summary>
    public class PatientService
    {
        private readonly AppointmentRepository _appointmentRepository;

        public PatientService(AppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public IList<Appointment> GetPatientAppointments(string patientName)
        {
            return _appointmentRepository.GetAppointmentsByPatient(patientName);
        }

        public void CancelPatientAppointment(int appointmentId, string patientName)
        {
            _appointmentRepository.CancelAppointment(appointmentId, patientName);
        }

        public void BookNewAppointment(string patientName, string doctorName, DateTime date, string time, string symptoms)
        {
            _appointmentRepository.BookAppointment(patientName, doctorName, date, time, symptoms);
        }
    }
}

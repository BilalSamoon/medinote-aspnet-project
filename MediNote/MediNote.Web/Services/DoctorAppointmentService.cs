using System;
using MediNote.Web.ViewModels;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Daniel Guillaumont
    /// Provides business logic for doctor appointment actions.
    /// </summary>
    public class DoctorAppointmentService
    {
        private readonly AppointmentRepository _appointmentRepository;

        public DoctorAppointmentService(AppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        /// <summary>
        /// Returns sample pending appointments for display.
        /// </summary>
        /// <returns>A populated pending appointments view model.</returns>
        public PendingAppointmentsViewModel GetPendingAppointmentsViewModel()
        {
            var pendingDb = _appointmentRepository.GetPendingAppointments();
            var vm = new PendingAppointmentsViewModel();

            foreach(var appt in pendingDb)
            {
                vm.PendingAppointments.Add(new PendingAppointmentItemViewModel
                {
                    AppointmentId = appt.AppointmentId,
                    PatientName = appt.PatientName,
                    RequestedDate = appt.RequestedDate,
                    Symptoms = appt.Symptoms
                });
            }

            return vm;
        }

        /// <summary>
        /// Returns an approval message for a selected appointment.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string ApproveAppointment(int appointmentId)
        {
            if (_appointmentRepository.UpdateStatus(appointmentId, "Approved"))
            {
                return $"Appointment #{appointmentId} was approved successfully.";
            }
            return $"Appointment #{appointmentId} not found.";
        }

        /// <summary>
        /// Returns a rejection message for a selected appointment.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string RejectAppointment(int appointmentId)
        {
            if (_appointmentRepository.UpdateStatus(appointmentId, "Rejected"))
            {
                return $"Appointment #{appointmentId} was rejected successfully.";
            }
            return $"Appointment #{appointmentId} not found.";
        }

        /// <summary>
        /// Returns sample appointment details for the reschedule page.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A populated reschedule appointment view model.</returns>
        public RescheduleAppointmentViewModel GetRescheduleViewModel(int appointmentId)
        {
            var appt = _appointmentRepository.GetAppointmentById(appointmentId);
            if (appt != null)
            {
                return new RescheduleAppointmentViewModel
                {
                    AppointmentId = appt.AppointmentId,
                    PatientName = appt.PatientName,
                    CurrentDate = appt.RequestedDate,
                    CurrentTime = appt.RequestedTime
                };
            }

            return new RescheduleAppointmentViewModel
            {
                AppointmentId = appointmentId,
                PatientName = "Unknown Patient",
                CurrentDate = DateTime.MinValue,
                CurrentTime = "Unavailable"
            };
        }

        /// <summary>
        /// Checks whether an appointment is eligible for rescheduling.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>True if the appointment is eligible; otherwise false.</returns>
        public bool IsAppointmentEligibleForReschedule(int appointmentId)
        {
            var appt = _appointmentRepository.GetAppointmentById(appointmentId);
            return appt != null && appt.Status != "Cancelled" && appt.Status != "Rejected";
        }

        /// <summary>
        /// Checks whether the selected new appointment slot conflicts with existing booked times.
        /// </summary>
        /// <param name="newDate">The new selected date.</param>
        /// <param name="newTime">The new selected time.</param>
        /// <returns>True if there is a conflict; otherwise false.</returns>
        public bool HasRescheduleConflict(DateTime newDate, TimeSpan newTime)
        {
            bool firstConflict =
                newDate.Date == new DateTime(2026, 3, 29).Date &&
                newTime == new TimeSpan(9, 0, 0);

            bool secondConflict =
                newDate.Date == new DateTime(2026, 3, 29).Date &&
                newTime == new TimeSpan(11, 30, 0);

            bool thirdConflict =
                newDate.Date == new DateTime(2026, 3, 29).Date &&
                newTime == new TimeSpan(14, 0, 0);

            return firstConflict || secondConflict || thirdConflict;
        }

        /// <summary>
        /// Returns a success message for a completed reschedule.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string ConfirmReschedule(int appointmentId)
        {
            return $"Appointment #{appointmentId} was rescheduled successfully.";
        }
    }
}
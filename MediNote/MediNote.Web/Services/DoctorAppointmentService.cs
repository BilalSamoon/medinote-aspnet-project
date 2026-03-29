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
        /// <summary>
        /// Returns sample pending appointments for display.
        /// </summary>
        /// <returns>A populated pending appointments view model.</returns>
        public PendingAppointmentsViewModel GetPendingAppointmentsViewModel()
        {
            return new PendingAppointmentsViewModel
            {
                PendingAppointments =
                {
                    new PendingAppointmentItemViewModel
                    {
                        AppointmentId = 1,
                        PatientName = "Emma Wilson",
                        RequestedDate = new DateTime(2026, 3, 30),
                        Symptoms = "Headache and fever"
                    },
                    new PendingAppointmentItemViewModel
                    {
                        AppointmentId = 2,
                        PatientName = "David Brown",
                        RequestedDate = new DateTime(2026, 3, 31),
                        Symptoms = "Chest discomfort"
                    }
                }
            };
        }

        /// <summary>
        /// Returns an approval message for a selected appointment.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string ApproveAppointment(int appointmentId)
        {
            return $"Appointment #{appointmentId} was approved successfully.";
        }

        /// <summary>
        /// Returns a rejection message for a selected appointment.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A success message.</returns>
        public string RejectAppointment(int appointmentId)
        {
            return $"Appointment #{appointmentId} was rejected successfully.";
        }

        /// <summary>
        /// Returns sample appointment details for the reschedule page.
        /// </summary>
        /// <param name="appointmentId">The appointment ID.</param>
        /// <returns>A populated reschedule appointment view model.</returns>
        public RescheduleAppointmentViewModel GetRescheduleViewModel(int appointmentId)
        {
            if (appointmentId == 1)
            {
                return new RescheduleAppointmentViewModel
                {
                    AppointmentId = 1,
                    PatientName = "Emma Wilson",
                    CurrentDate = new DateTime(2026, 3, 30),
                    CurrentTime = "10:00 AM"
                };
            }

            if (appointmentId == 2)
            {
                return new RescheduleAppointmentViewModel
                {
                    AppointmentId = 2,
                    PatientName = "David Brown",
                    CurrentDate = new DateTime(2026, 3, 31),
                    CurrentTime = "1:30 PM"
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
            return appointmentId == 1 || appointmentId == 2;
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
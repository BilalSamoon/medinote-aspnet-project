using System.Linq;
using MediNote.Web.Contracts;
using MediNote.Web.Data;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    //By: Bilal 
    // API controller for admin-related operations, including dashboard data retrieval, appointment management, doctor slot information, and user account generation. Access to this controller is restricted to users with the "Admin" role.s
    [ApiController]
  
    [Route("api/admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly MediNoteDbContext _context;
        private readonly PatientService _patientService;
        private readonly DoctorAppointmentService _doctorAppointmentService;
        private readonly UserRepository _userRepository;

        public AdminApiController(MediNoteDbContext context, PatientService patientService, DoctorAppointmentService doctorAppointmentService, UserRepository userRepository)
        {
            _context = context;
            _patientService = patientService;
            _doctorAppointmentService = doctorAppointmentService;
            _userRepository = userRepository;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var dto = new AdminDashboardSummaryDto
            {
                TotalAppointments = _context.Appointments.Count(),
                PendingAppointments = _context.Appointments.Count(a => a.Status == "Pending"),
                ApprovedAppointments = _context.Appointments.Count(a => a.Status == "Approved"),
                CancelledAppointments = _context.Appointments.Count(a => a.Status == "Cancelled"),
                CompletedAppointments = _context.Appointments.Count(a => a.Status == "Completed"),
                RecentAppointments = _context.Appointments
                    .OrderByDescending(a => a.RequestedDate)
                    .ThenBy(a => a.RequestedTime)
                    .Take(10)
                    .Select(a => new AppointmentSummaryDto
                    {
                        AppointmentId = a.AppointmentId,
                        PatientName = a.PatientName,
                        DoctorName = a.DoctorName,
                        RequestedDate = a.RequestedDate,
                        RequestedTime = a.RequestedTime,
                        Symptoms = a.Symptoms,
                        Status = a.Status,
                        CanCancel = a.Status == "Pending" || a.Status == "Approved",
                        HasDoctorNotes = _context.DoctorNotes.Any(n => n.AppointmentId == a.AppointmentId),
                        HasPrescriptions = _context.Prescriptions.Any(p => p.AppointmentId == a.AppointmentId)
                    })
                    .ToList()
            };

            return Ok(dto);
        }

        [HttpGet("appointments")]
        public IActionResult GetAppointments([FromQuery] string? status = null)
        {
            var query = _context.Appointments.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            return Ok(query.OrderByDescending(a => a.RequestedDate).ThenBy(a => a.RequestedTime).ToList());
        }

        [HttpPost("appointments")]
        public IActionResult CreateAppointment([FromBody] BookAppointmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PatientName))
            {
                return BadRequest(new { message = "Patient name is required when an admin creates an appointment." });
            }

            var ok = _patientService.TryBookNewAppointment(
                request.PatientName,
                request.DoctorName,
                request.RequestedDate,
                request.RequestedTime,
                request.Symptoms,
                request.ContactRecipient,
                request.NotificationChannel,
                out var appointment,
                out var errorMessage);

            if (!ok || appointment == null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Appointment created successfully.", appointmentId = appointment.AppointmentId });
        }

        [HttpPost("appointments/{id:int}/approve")]
        public IActionResult ApproveAppointment(int id)
        {
            return Ok(new { message = _doctorAppointmentService.ApproveAppointment(id) });
        }

        [HttpPost("appointments/{id:int}/cancel")]
        public IActionResult CancelAppointment(int id)
        {
            return Ok(new { message = _doctorAppointmentService.CancelAppointment(id) });
        }

        [HttpGet("doctor-slots")]
        public IActionResult GetDoctorSlots([FromQuery] string? doctorName = null)
        {
            return Ok(_patientService.GetDoctorSlots(doctorName));
        }

        [HttpGet("bookable-slots")]
        public IActionResult GetBookableSlots([FromQuery] string? doctorName = null)
        {
            return Ok(_patientService.GetBookableSlotOptions(doctorName));
        }

        [HttpPost("users/generate")]
        public IActionResult GenerateUser([FromBody] MediNote.Web.ViewModels.AdminGenerateUserViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = _userRepository.RegisterUser(
                request.FirstName,
                request.LastName,
                request.Username,
                request.Password,
                request.Role,
                string.Empty,
                request.Email,
                out var errorMessage,
                out var issuedSecurityId,
                isAdminAction: true);

            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new
            {
                message = $"Account for {request.Role} '{request.FirstName} {request.LastName}' created successfully.",
                issuedSecurityId
            });
        }

        [HttpPost("security-codes/generate")]
        public IActionResult GenerateSecurityCode([FromBody] string role)
        {
            var code = _userRepository.GenerateSecurityCodeForRole(role);
            return Ok(new { message = $"Security code generated for {role}.", code });
        }
    }
}

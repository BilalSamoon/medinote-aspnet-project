using MediNote.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.API.Controllers
{
    /// <summary>
    /// Author: Bilal Ahmed Samoon
    /// Controller responsible for admin-related API endpoints such as retrieving
    /// system statistics and all appointment data.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly MediNoteDbContext _context;

        public AdminApiController(MediNoteDbContext context)
        {
            _context = context;
        }

        // Get statistics 
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            return Ok(new
            {
                total = _context.Appointments.Count(),
                pending = _context.Appointments.Count(a => a.Status == "Pending")
            });
        }

        // Get ALL appointments
        [HttpGet("all")]
        public IActionResult GetAllAppointments()
        {
            var data = _context.Appointments.ToList();
            return Ok(data);
        }
    }
}
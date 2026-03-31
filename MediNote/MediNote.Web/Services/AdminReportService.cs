using System.Linq;
using MediNote.Web.Data;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Author: Bilal Ahmed Samoon
    /// Provides business logic for generating system reports based on SQL data.
    /// </summary>
    public class AdminReportService
    {
        private readonly MediNoteDbContext _context;

        public AdminReportService(MediNoteDbContext context)
        {
            _context = context;
        }

        public int GetTotalAppointments(int count)
        {
            return count;
        }

        public int GetPendingAppointments(int count)
        {
            return count;
        }
    }
}
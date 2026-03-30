using Microsoft.EntityFrameworkCore;
using MediNote.Web.Models;

namespace MediNote.Web.Data
{
    /// Author: Bilal Ahmed Samoon
    /// Represents the database context for MediNote using EF Core.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
    }
}
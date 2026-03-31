using MediNote.Web.Models;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// Author: Camila Esguerra
/// Db context for the MediNote application, responsible for managing database access and providing DbSet properties for each entity in the application.
/// </summary>

namespace MediNote.Web.Data
{
    public class MediNoteDbContext : DbContext
    {
        public MediNoteDbContext(DbContextOptions<MediNoteDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Availability> Availabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed some default users so you still have some initial accounts to log in with
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "doctor1", Password = "password123", Role = "Doctor", SecurityId = "DOC123" },
                new User { Id = 2, Username = "admin1", Password = "adminpassword", Role = "Admin", SecurityId = "ADM123" },
                new User { Id = 3, Username = "patient1", Password = "patientpassword", Role = "Patient" }
            );
        }
    }
}
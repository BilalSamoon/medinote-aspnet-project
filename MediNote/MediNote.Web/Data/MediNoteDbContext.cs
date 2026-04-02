using MediNote.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MediNote.Web.Data
{
    /// <summary>
    /// Db context for the MediNote application.
    /// </summary>
    public class MediNoteDbContext : DbContext
    {
        public MediNoteDbContext(DbContextOptions<MediNoteDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<SecurityCode> SecurityCodes { get; set; }
        public DbSet<DoctorNote> DoctorNotes { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "doctor1",
                    Password = "password123",
                    Role = "Doctor",
                    SecurityId = "DOC123",
                    Email = "doctor1@medinote.local"
                },
                new User
                {
                    Id = 2,
                    FirstName = "Alice",
                    LastName = "Admin",
                    Username = "admin1",
                    Password = "adminpassword",
                    Role = "Admin",
                    SecurityId = "ADM123",
                    Email = "admin1@medinote.local"
                },
                new User
                {
                    Id = 3,
                    FirstName = "Bob",
                    LastName = "Patient",
                    Username = "patient1",
                    Password = "patientpassword",
                    Role = "Patient",
                    SecurityId = string.Empty,
                    Email = "patient1@medinote.local"
                });

            modelBuilder.Entity<SecurityCode>().HasData(
                new SecurityCode { Id = 1, Code = "DOC123", Role = "Doctor", IsClaimed = true },
                new SecurityCode { Id = 2, Code = "ADM123", Role = "Admin", IsClaimed = true },
                new SecurityCode { Id = 3, Code = "ADM456", Role = "Admin", IsClaimed = false },
                new SecurityCode { Id = 4, Code = "ADM789", Role = "Admin", IsClaimed = false });
        }
    }
}

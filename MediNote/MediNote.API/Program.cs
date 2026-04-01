using MediNote.Web.Data;
using MediNote.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace MediNote.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //  Controllers
            builder.Services.AddControllers();

       
            builder.Services.AddOpenApi();

            //  Database
            builder.Services.AddDbContext<MediNoteDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Services
            builder.Services.AddScoped<DoctorAppointmentService>();
            builder.Services.AddScoped<ScheduleService>();
            builder.Services.AddScoped<AvailabilityService>();
            builder.Services.AddScoped<PatientService>();
            builder.Services.AddScoped<PriorityCalculationService>();
            builder.Services.AddScoped<AdminReportService>();
            builder.Services.AddScoped<AppointmentRepository>();
            builder.Services.AddScoped<UserRepository>();

            var app = builder.Build();

    
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
using MediNote.Web.Services;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;

namespace MediNote.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ScheduleService>();
            builder.Services.AddScoped<AvailabilityService>();
            builder.Services.AddScoped<DoctorAppointmentService>();
            builder.Services.AddScoped<PriorityCalculationService>();
            builder.Services.AddScoped<AdminReportService>();

            // Add DbContext. by: camila esguerra
            builder.Services.AddDbContext<MediNoteDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //specific path is in appsettings.json

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
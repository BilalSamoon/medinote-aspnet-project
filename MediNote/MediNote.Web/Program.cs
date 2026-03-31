using MediNote.Web.Services;
using Microsoft.EntityFrameworkCore;
using MediNote.Web.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MediNote.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(options => 
            {
                options.RootDirectory = "/Views";
            });
            builder.Services.AddScoped<ScheduleService>();
            builder.Services.AddScoped<AvailabilityService>();
            builder.Services.AddScoped<DoctorAppointmentService>();
            builder.Services.AddScoped<PatientService>();
            builder.Services.AddScoped<PriorityCalculationService>();
            builder.Services.AddScoped<AdminReportService>();
            builder.Services.AddScoped<UserRepository>(); // Added missing DI
            builder.Services.AddScoped<AppointmentRepository>(); // Added missing DI

            // Add DbContext. by: camila esguerra
            builder.Services.AddDbContext<MediNoteDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //specific path is in appsettings.json

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
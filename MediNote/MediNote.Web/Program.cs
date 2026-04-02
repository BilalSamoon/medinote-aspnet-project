using MediNote.Web.Data;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace MediNote.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<AppointmentRepository>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<DoctorPortalService>();

            builder.Services.AddDbContext<MediNoteDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MediNoteDbContext>();
                SchemaBootstrapper.EnsureCompatibleSchema(dbContext);
            }

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

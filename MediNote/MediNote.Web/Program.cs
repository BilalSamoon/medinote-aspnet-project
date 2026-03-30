using MediNote.Web.Services;
using MediNote.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace MediNote.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext. by: camila esguerra
            builder.Services.AddDbContext<MediNoteDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(options =>
            {
                options.RootDirectory = "/Views";
            }); // Added for Razor Pages support (like the Login page)

            // Add Cookie Authentication
            builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddScoped<ScheduleService>();
            builder.Services.AddScoped<MediNote.Web.Services.UserRepository>(); // scoped for ef core integration
            builder.Services.AddScoped<MediNote.Web.Services.AppointmentRepository>(); // scoped for ef core integration
            builder.Services.AddScoped<AvailabilityService>();
            builder.Services.AddScoped<DoctorAppointmentService>();
            builder.Services.AddScoped<PatientService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication(); // Ensure Authentication is triggered before Authorization
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages(); // Map Razor Pages routing

            // Ensures database is created
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MediNoteDbContext>();
                dbContext.Database.Migrate(); //  using migrations
            }

            app.Run();
        }
    }
}
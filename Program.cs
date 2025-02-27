using BlazorAuth2.Areas.Identity;
using BlazorAuth2.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace BlazorAuth2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddHttpContextAccessor();



          
            var appointmentConnectionString = builder.Configuration.GetConnectionString("AppointmentConnection")
            ?? throw new InvalidOperationException("Connection string 'AppointmentConnection' not found.");

            builder.Services.AddDbContext<AppointmentDbContext>(options =>
                options.UseSqlServer(appointmentConnectionString));

            builder.Services.AddScoped<AppointmentService>();

            var app = builder.Build();

          
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

         
            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");  

            app.Run();
        }
    }
}



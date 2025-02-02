using Microsoft.EntityFrameworkCore;

namespace BlazorAuth2.Data  // Obavezno promijeni namespace na novi projekt!
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BlazorAuth2.Data
{
    public class AppointmentService
    {
        private readonly AppointmentDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentService(AppointmentDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Dohvati sve termine iz baze
        /// </summary>
        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        /// <summary>
        /// Dohvati termine prijavljenog korisnika
        /// </summary>
        public async Task<List<Appointment>> GetUserAppointmentsAsync()
        {
            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name?.Split('@')[0];
            if (string.IsNullOrEmpty(userName))
            {
                return new List<Appointment>(); // Vraæa prazan popis ako korisnik nije prijavljen
            }

            return await _context.Appointments
                .Where(a => a.CreatedBy == userName)
                .ToListAsync();
        }

        /// <summary>
        /// Dodaj novi termin u bazu
        /// </summary>
        public async Task<bool> BookAppointmentAsync(Appointment appointment)
        {
            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name?.Split('@')[0];

            if (string.IsNullOrEmpty(userName))
            {
                return false; // Ako korisnik nije prijavljen, ne dozvoljavamo zakazivanje
            }

            appointment.CreatedBy = userName; // OVDJE osiguravamo da uvijek bude postavljen korisnik

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }



        /// <summary>
        /// Otkazivanje termina
        /// </summary>
        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return false; // Termin nije pronaðen
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

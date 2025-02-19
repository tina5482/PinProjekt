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
        /// Gets all appointments from the database.
        /// </summary>
        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        /// <summary>
        /// Gets appointments for the currently logged-in user.
        /// </summary>
        public async Task<List<Appointment>> GetUserAppointmentsAsync()
        {
            var userName = GetCurrentUser();
            if (string.IsNullOrEmpty(userName))
            {
                return new List<Appointment>();
            }

            return await _context.Appointments
                .Where(a => a.CreatedBy == userName)
                .ToListAsync();
        }

        /// <summary>
        /// Books a new appointment while preventing past, Sunday, and overlapping bookings.
        /// </summary>
        public async Task<bool> BookAppointmentAsync(Appointment appointment)
        {
            var userName = GetCurrentUser();
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            appointment.CreatedBy = userName;

            // Prevent booking in the past or on Sundays
            if (appointment.AppointmentDateTime < DateTime.Now || appointment.AppointmentDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            int serviceDuration = GetServiceDuration(appointment.ServiceType);
            DateTime appointmentEnd = appointment.AppointmentDateTime.AddMinutes(serviceDuration);

            // Fetch existing appointments first
            var userAppointments = await _context.Appointments
                .Where(a => a.CreatedBy == userName && a.Id != appointment.Id)
                .ToListAsync();

            // Check for overlap in memory
            bool hasOverlap = userAppointments.Any(a =>
                appointment.AppointmentDateTime < a.AppointmentDateTime.AddMinutes(GetServiceDuration(a.ServiceType)) &&
                appointmentEnd > a.AppointmentDateTime
            );

            if (hasOverlap)
            {
                return false;
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates an existing appointment while ensuring no overlapping.
        /// </summary>
        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            var existingAppointment = await _context.Appointments.FindAsync(appointment.Id);
            if (existingAppointment == null)
            {
                return false;
            }

            if (appointment.AppointmentDateTime < DateTime.Now || appointment.AppointmentDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            int serviceDuration = GetServiceDuration(appointment.ServiceType);
            DateTime appointmentEnd = appointment.AppointmentDateTime.AddMinutes(serviceDuration);

            var userAppointments = await _context.Appointments
                .Where(a => a.CreatedBy == existingAppointment.CreatedBy && a.Id != existingAppointment.Id)
                .ToListAsync();

            bool hasOverlap = userAppointments.Any(a =>
                appointment.AppointmentDateTime < a.AppointmentDateTime.AddMinutes(GetServiceDuration(a.ServiceType)) &&
                appointmentEnd > a.AppointmentDateTime
            );

            if (hasOverlap)
            {
                return false;
            }

            existingAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
            _context.Appointments.Update(existingAppointment);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates the notes for an existing appointment.
        /// </summary>
        public async Task<bool> UpdateAppointmentNotesAsync(int appointmentId, string notes)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Notes = notes;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return false;
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        private int GetServiceDuration(string serviceType)
        {
            return serviceType switch
            {
                "Šminkanje" => 120,
                "Lash Lift" => 60,
                "Brow Lift" => 60,
                "Ekstenzije trepavica" => 90,
                _ => 60
            };
        }

        private string GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.Name?.Split('@')[0] ?? string.Empty;
        }
    }
}

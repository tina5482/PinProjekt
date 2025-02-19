using System.ComponentModel.DataAnnotations;

namespace BlazorAuth2.Data
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        //  New Notes field (optional)
        public string? Notes { get; set; } // Can be empty
    }
}

using System.ComponentModel.DataAnnotations;

namespace BlazorCinemaBooking.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; } = String.Empty;

    }
}

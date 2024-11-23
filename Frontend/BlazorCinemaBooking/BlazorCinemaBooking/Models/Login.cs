using System.ComponentModel.DataAnnotations;

namespace BlazorCinemaBooking.Models
{
    public class Login
    {
        [Required]
        public string Username { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; } = String.Empty;

    }
}

using SharedLibrary.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeatReservationService.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string UserEmail { get; set; }

        [Required]
        public int ShowtimeId { get; set; }
        [NotMapped]
        public ShowtimeDTO? Showtime { get; set; }

        [Required]
        public List<ReservedSeat> ReservedSeats { get; set; } = new List<ReservedSeat>();
    }
}

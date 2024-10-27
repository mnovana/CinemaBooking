using SharedLibrary.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SeatReservationService.Models
{
    public class ReservedSeat
    {
        [Required]
        public int ReservationId { get; set; }
        [JsonIgnore]
        public Reservation? Reservation { get; set; }
        
        [Required]
        public int SeatId { get; set; }
        [NotMapped]
        public SeatDTO? Seat { get; set; }
    }
}

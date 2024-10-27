using SharedLibrary.Models.DTO;

namespace SeatReservationService.Models.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public ShowtimeDTO Showtime { get; set; }
        public List<ReservedSeatDTO> ReservedSeats { get; set; } = new List<ReservedSeatDTO>();
    }
}

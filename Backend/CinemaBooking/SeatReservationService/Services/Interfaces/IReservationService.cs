using SeatReservationService.Models;
using SeatReservationService.Models.DTO;

namespace SeatReservationService.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllAsync();
        Task<ReservationDTO?> GetByIdAsync(int id);
        Task<ReservationDTO> AddAsync(Reservation reservation);
        Task<ReservationDTO?> UpdateAsync(Reservation reservation);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReservationWithShowtimeIdExistsAsync(int showtimeId);
    }
}

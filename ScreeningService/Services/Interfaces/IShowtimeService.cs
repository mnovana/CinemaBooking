using ScreeningService.Models;
using SharedLibrary.Models.DTO;

namespace ScreeningService.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<IEnumerable<ShowtimeDTO>> GetAllAsync();
        Task<ShowtimeDTO?> GetById(int id); 
        Task<ShowtimeDTO> AddAsync(Showtime showtime);
        Task<ShowtimeDTO?> UpdateAsync(Showtime showtime);
        Task<bool> DeleteAsync(int id);
    }
}

using ScreeningService.Models;
using SharedLibrary.Models.DTO;

namespace ScreeningService.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<IEnumerable<ShowtimeDTO>> GetAllAsync();
        Task<ShowtimeDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ShowtimeDTO>> GetByIdsAsync(int[] ids);
        Task<ShowtimeDTO> AddAsync(Showtime showtime);
        Task<ShowtimeDTO?> UpdateAsync(Showtime showtime);
        Task<bool> DeleteAsync(int id);
        Task<bool> ShowtimeWithMovieIdExistsAsync(int movieId);
    }
}

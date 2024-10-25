using ScreeningService.Models;

namespace ScreeningService.Repositories.Interfaces
{
    public interface IShowtimeRepository
    {
        Task<IEnumerable<Showtime>> GetAllAsync();
        Task<Showtime?> GetByIdAsync(int id);
        Task AddAsync(Showtime showtime);
        Task<bool> UpdateAsync(Showtime showtime);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Showtime>> GetByMovieId(int movieId);
        Task<IEnumerable<Showtime>> GetByDate(DateTime date);
        Task<bool> ShowtimesOverlap(Showtime showtime);
    }
}

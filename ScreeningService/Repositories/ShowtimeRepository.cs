using Microsoft.EntityFrameworkCore;
using ScreeningService.Models;
using ScreeningService.Repositories.Interfaces;

namespace ScreeningService.Repositories
{
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly AppDbContext _context;

        public ShowtimeRepository(AppDbContext context)
        { 
            _context = context; 
        
        }

        public async Task AddAsync(Showtime showtime)
        {
            _context.Add(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int rowsAffected = await _context.Showtimes
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();

            return rowsAffected == 1;
        }

        public async Task<IEnumerable<Showtime>> GetAllAsync()
        {
            return await _context.Showtimes
                .Include(s => s.ScreeningRoom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetByDate(DateTime date)
        {
            var startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 0);

            return await _context.Showtimes
                .Include(s => s.ScreeningRoom)
                .Where(s => s.Start >= startOfDay && s.Start <= endOfDay)
                .ToListAsync();
        }

        public async Task<Showtime?> GetByIdAsync(int id)
        {
            return await _context.Showtimes
                .Include(s => s.ScreeningRoom)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Showtime>> GetByIdsAsync(int[] ids)
        {
            return await _context.Showtimes
                .Include(s => s.ScreeningRoom)
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetByMovieId(int movieId)
        {
            return await _context.Showtimes
                .Include(s => s.ScreeningRoom)
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }

        public Task<bool> ShowtimesOverlap(Showtime showtime)
        {
            return _context.Showtimes
                .AnyAsync(s => s.ScreeningRoomId == showtime.ScreeningRoomId &&
                               s.Start <= showtime.End && 
                               s.End >= showtime.Start
                );
        }

        public async Task<bool> ShowtimeWithMovieIdExistsAsync(int movieId)
        {
            return await _context.Showtimes.AnyAsync(s => s.MovieId == movieId);
        }

        public async Task<bool> UpdateAsync(Showtime showtime)
        {
            _context.Entry(showtime).State = EntityState.Modified;

            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}

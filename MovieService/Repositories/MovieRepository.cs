using Microsoft.EntityFrameworkCore;
using MovieService.Models;
using SharedLibrary.Models.DTO;
using MovieService.Repositories.Interfaces;

namespace MovieService.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context;

        public MovieRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task AddAsync(Movie movie)
        {
            _context.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int rowsAffected = await _context.Movies
                .Where(m => m.Id == id)
                .ExecuteDeleteAsync();

            return rowsAffected == 1;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MovieTitleDurationDTO?> GetTitleDurationByIdAsync(int id)
        {
            return await _context.Movies
                .Select(m => new MovieTitleDurationDTO { Id = m.Id, Title = m.Title, Duration = m.Duration })
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MovieTitleDTO>> GetTitlesByIdsAsync(List<int> ids)
        {
            return await _context.Movies
                .Where(m => ids.Contains(m.Id))
                .Select(m => new MovieTitleDTO { Id = m.Id, Title = m.Title })
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            
            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}

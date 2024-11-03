using Microsoft.EntityFrameworkCore;
using FilmService.Models;
using FilmService.Repositories.Interfaces;

namespace FilmService.Repositories
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly AppDbContext _context;

        public DirectorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Director director)
        {
            await _context.Directors.AddAsync(director);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int rowsAffected = await _context.Directors
                .Where(d => d.Id == id)
                .ExecuteDeleteAsync();

            return rowsAffected == 1;
        }

        public async Task<IEnumerable<Director>> GetAllAsync()
        {
            return await _context.Directors.ToListAsync();
        }

        public async Task<Director?> GetByIdAsync(int id)
        {
            return await _context.Directors.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> UpdateAsync(Director director)
        {
            bool exists = await _context.Directors.AnyAsync(d => d.Id == director.Id);

            if (!exists)
            {
                return false;
            }

            _context.Entry(director).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ScreeningService.Models;
using ScreeningService.Repositories.Interfaces;

namespace ScreeningService.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetAllAsync()
        {
            return await _context.Seats
                .Include(s => s.Row)
                .ThenInclude(r => r.ScreeningRoom)
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .Include(s => s.Row)
                .ThenInclude(r => r.ScreeningRoom)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Seat>> GetByIdsAsync(int[] ids)
        {
            return await _context.Seats
                .Include(s => s.Row)
                .ThenInclude(r => r.ScreeningRoom)
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }
    }
}

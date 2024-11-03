using Microsoft.EntityFrameworkCore;
using SeatReservationService.Models;
using SeatReservationService.Repositories.Interfaces;

namespace SeatReservationService.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task AddAsync(Reservation reservation)
        {
            _context.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int rowsAffected = await _context.Reservations
                .Where(r => r.Id == id)
                .ExecuteDeleteAsync();

            return rowsAffected == 1;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.ReservedSeats)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.ReservedSeats)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<int>> GetTakenSeatsAsync(int showtimeId, int[] seatsIds)
        {
            return await _context.ReservedSeats
                .Include(rs => rs.Reservation)
                .Where(rs => rs.Reservation.ShowtimeId == showtimeId && seatsIds.Contains(rs.SeatId))
                .Select(rs => rs.SeatId)
                .ToArrayAsync();
        }

        public async Task<bool> UpdateAsync(Reservation reservation)
        {
            // delete existing ReservedSeat rows
            await _context.ReservedSeats
                .Where(rs => rs.ReservationId == reservation.Id)
                .ExecuteDeleteAsync();

            // add new ReservedSeat rows
            foreach (var reservedSeat in reservation.ReservedSeats)
            {
                _context.ReservedSeats.Add(reservedSeat);
            }

            _context.Entry(reservation).State = EntityState.Modified;

            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}

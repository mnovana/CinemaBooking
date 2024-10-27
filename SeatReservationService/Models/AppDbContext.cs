using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models.DTO;

namespace SeatReservationService.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservedSeat> ReservedSeats { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key
            modelBuilder.Entity<ReservedSeat>()
                .HasKey(r => new { r.ReservationId, r.SeatId });

            // Cascade delete
            modelBuilder.Entity<ReservedSeat>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservedSeats)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation { Id = 1, ShowtimeId = 1, UserEmail = "novana@gmail.com" },
                new Reservation { Id = 2, ShowtimeId = 2, UserEmail = "novana@gmail.com" },
                new Reservation { Id = 3, ShowtimeId = 3, UserEmail = "novana@gmail.com" },
                new Reservation { Id = 4, ShowtimeId = 4, UserEmail = "novana@gmail.com" }
            );

            modelBuilder.Entity<ReservedSeat>().HasData(
                new ReservedSeat { ReservationId = 1, SeatId = 1 },
                new ReservedSeat { ReservationId = 1, SeatId = 2 },
                new ReservedSeat { ReservationId = 2, SeatId = 1 },
                new ReservedSeat { ReservationId = 2, SeatId = 2 },
                new ReservedSeat { ReservationId = 3, SeatId = 14 },
                new ReservedSeat { ReservationId = 3, SeatId = 15 },
                new ReservedSeat { ReservationId = 4, SeatId = 14 }
            );
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace ScreeningService.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<ScreeningRoom> ScreeningRooms { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<ScreeningRoom>()
                .HasIndex(s => s.Number)
                .IsUnique();

            modelBuilder.Entity<Row>()
                .HasIndex(r => new { r.Number, r.ScreeningRoomId })
                .IsUnique();

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.Number, s.RowId })
                .IsUnique();

            // End is required for database and optional for data model
            modelBuilder.Entity<Showtime>()
                .Property(s => s.End)
                .IsRequired();

            // Seed data
            modelBuilder.Entity<ScreeningRoom>().HasData(
                new ScreeningRoom { Id = 1, Number = 1 },
                new ScreeningRoom { Id = 2, Number = 2 }
            );

            modelBuilder.Entity<Row>().HasData(
                new Row { Id = 1, Number = 1, ScreeningRoomId = 1 },
                new Row { Id = 2, Number = 2, ScreeningRoomId = 1 },
                new Row { Id = 3, Number = 3, ScreeningRoomId = 1 },
                new Row { Id = 4, Number = 1, ScreeningRoomId = 2 },
                new Row { Id = 5, Number = 2, ScreeningRoomId = 2 },
                new Row { Id = 6, Number = 3, ScreeningRoomId = 2 }
            );

            modelBuilder.Entity<Seat>().HasData(
                GetSeats()
            );

            modelBuilder.Entity<Showtime>().HasData(
                new Showtime { Id = 1, Start = new DateTime(2024, 10, 10, 17, 0, 0), End = new DateTime(2024, 10, 10, 19, 33, 0),  ScreeningRoomId = 1, MovieId = 1 },
                new Showtime { Id = 2, Start = new DateTime(2024, 10, 10, 21, 30, 0), End = new DateTime(2024, 10, 11, 0, 3, 0), ScreeningRoomId = 1, MovieId = 1 },
                new Showtime { Id = 3, Start = new DateTime(2024, 10, 10, 17, 0, 0), End = new DateTime(2024, 10, 10, 18, 51, 0), ScreeningRoomId = 2, MovieId = 2 },
                new Showtime { Id = 4, Start = new DateTime(2024, 10, 10, 21, 30, 0), End = new DateTime(2024, 10, 10, 23, 21, 0), ScreeningRoomId = 2, MovieId = 2 },
                new Showtime { Id = 5, Start = new DateTime(2024, 10, 11, 17, 0, 0), End = new DateTime(2024, 10, 10, 18, 53, 0), ScreeningRoomId = 1, MovieId = 3 },
                new Showtime { Id = 6, Start = new DateTime(2024, 10, 11, 17, 0, 0), End = new DateTime(2024, 10, 10, 18, 53, 0), ScreeningRoomId = 2, MovieId = 3 }
            );
        }

        private List<Seat> GetSeats()
        {
            List<Seat> seats = new List<Seat>();
            int id = 1;

            for (int row = 1; row <= 6; row++)
            {
                for (int num = 1; num <= 4; num++)
                { 
                    seats.Add(new Seat { Id = id, Number = num, RowId = row });
                    id++;
                }
            }

            return seats;
        }
    }
}

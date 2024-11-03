using Microsoft.EntityFrameworkCore;

namespace FilmService.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Director> Directors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            modelBuilder.Entity<Director>().HasData(
                new Director { Id = 1, Name = "Quentin Tarantino" },
                new Director { Id = 2, Name = "Christopher Nolan" },
                new Director { Id = 3, Name = "Denis Villeneuve" }
            );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Comedy" },
                new Genre { Id = 3, Name = "Drama" },
                new Genre { Id = 4, Name = "Horror" },
                new Genre { Id = 5, Name = "Science fiction" },
                new Genre { Id = 6, Name = "Crime" }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "Pulp Fiction", Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.", Duration = 153, ReleaseDate = new DateTime(1994, 10, 14), DirectorId = 1, GenreId = 6 },
                new Movie { Id = 2, Title = "Kill Bill: Volume 1", Description = "After waking from a 4-year coma, a former assassin wreaks vengeance on the team of assassins who betrayed her.", Duration = 111, ReleaseDate = new DateTime(2003, 11, 1), DirectorId = 1, GenreId = 1 },
                new Movie { Id = 3, Title = "Memento", Description = "Leonard Shelby, an insurance investigator, suffers from anterograde amnesia and uses notes and tattoos to hunt for the man he thinks killed his wife, which is the last thing he remembers.", Duration = 113, ReleaseDate = new DateTime(2001, 3, 16), DirectorId = 2, GenreId = 6 },
                new Movie { Id = 4, Title = "Tenet", Description = "Armed with only the word 'Tenet' and fighting for the survival of the entire world, CIA operative, The Protagonist, journeys through a twilight world of international espionage on a global mission that unfolds beyond real time.", Duration = 150, ReleaseDate = new DateTime(2020, 8, 26), DirectorId = 2, GenreId = 1 },
                new Movie { Id = 5, Title = "Dune", Description = "A noble family becomes embroiled in a war for control over the galaxy's most valuable asset while its heir becomes troubled by visions of a dark future.", Duration = 155, ReleaseDate = new DateTime(2021, 10, 22), DirectorId = 3, GenreId = 5 },
                new Movie { Id = 6, Title = "Blade Runner 2049", Description = "Young Blade Runner K's discovery of a long-buried secret leads him to track down former Blade Runner Rick Deckard, who's been missing for thirty years.", Duration = 163, ReleaseDate = new DateTime(2017, 10, 5), DirectorId = 3, GenreId = 5 }
            );

        }
    }
}

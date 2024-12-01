using FilmService.Controllers;
using FilmService.Models;
using FilmService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLibrary.Models.DTO;

namespace FilmService.UnitTests.Controllers
{
    public class MoviesControllerTests
    {
        [Fact]
        public async Task GetMovieById_ValidId_ReturnsObject()
        {
            // Arrange
            MovieDTO movieDto = new MovieDTO
            {
                Id = 1,
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Duration = 153,
                ReleaseDate = new DateTime(1994, 10, 14),
                DirectorName = "Quentin Tarantino",
                GenreName = "Crime"
            };

            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockService = new Mock<IMovieService>();
            mockService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(movieDto);

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.GetMovieById(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);

            var objectResult = (MovieDTO)result.Value;

            Assert.Equal(movieDto.Id, objectResult.Id);
            Assert.Equal(movieDto.Title, objectResult.Title);
            Assert.Equal(movieDto.Description, objectResult.Description);
            Assert.Equal(movieDto.Duration, objectResult.Duration);
            Assert.Equal(movieDto.ReleaseDate, objectResult.ReleaseDate);
            Assert.Equal(movieDto.DirectorName, objectResult.DirectorName);
            Assert.Equal(movieDto.GenreName, objectResult.GenreName);
        }

        [Fact]
        public async Task GetMovieById_InvalidId_ReturnsNotFound()
        {
            // Assert
            var mockService = new Mock<IMovieService>();
            var mockLogger = new Mock<ILogger<MoviesController>>();

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.GetMovieById(1) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteMovie_ValidId_ReturnsNoContent()
        {
            // Assert
            var mockService = new Mock<IMovieService>();
            mockService.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);
            var mockLogger = new Mock<ILogger<MoviesController>>();

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.DeleteMovie(1) as NoContentResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteMovie_InvalidId_ReturnsNotFound()
        {
            // Assert
            var mockService = new Mock<IMovieService>();
            mockService.Setup(x => x.DeleteAsync(1)).ReturnsAsync(false);
            var mockLogger = new Mock<ILogger<MoviesController>>();

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.DeleteMovie(1) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task PostMovie_ReturnsCreatedAtAction()
        {
            // Arrange
            Movie userMovie = new Movie
            {
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Duration = 153,
                ReleaseDate = new DateTime(1994, 10, 14),
                DirectorId = 1,
                GenreId = 6
            };

            MovieDTO createdMovie = new MovieDTO
            {
                Id = 1,
                Title = userMovie.Title,
                Description = userMovie.Description,
                Duration = userMovie.Duration,
                ReleaseDate = userMovie.ReleaseDate,
                DirectorName = "Quentin Tarantino",
                GenreName = "Crime"
            };

            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockService = new Mock<IMovieService>();
            mockService.Setup(x => x.AddAsync(userMovie))
                .Callback<Movie>(um => um.Id = createdMovie.Id)
                .ReturnsAsync(createdMovie); ;

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.PostMovie(userMovie) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal("GetMovieById", result.ActionName);
            Assert.Equal(1, result.RouteValues["id"]);

            var objectResult = (MovieDTO)result.Value;

            Assert.Equal(createdMovie.Id, objectResult.Id);
            Assert.Equal(createdMovie.Title, objectResult.Title);
            Assert.Equal(createdMovie.Description, objectResult.Description);
            Assert.Equal(createdMovie.Duration, objectResult.Duration);
            Assert.Equal(createdMovie.ReleaseDate, objectResult.ReleaseDate);
            Assert.Equal(createdMovie.DirectorName, objectResult.DirectorName);
            Assert.Equal(createdMovie.GenreName, objectResult.GenreName);
        }

        [Fact]
        public async Task PutMovie_ValidRequest_ReturnsOkObject()
        {
            // Arrange
            Movie userMovie = new Movie
            {
                Id = 1,
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Duration = 153,
                ReleaseDate = new DateTime(1994, 10, 14),
                DirectorId = 1,
                GenreId = 6
            };

            MovieDTO updatedMovie = new MovieDTO
            {
                Id = 1,
                Title = userMovie.Title,
                Description = userMovie.Description,
                Duration = userMovie.Duration,
                ReleaseDate = userMovie.ReleaseDate,
                DirectorName = "Quentin Tarantino",
                GenreName = "Crime"
            };

            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockService = new Mock<IMovieService>();
            mockService.Setup(x => x.UpdateAsync(userMovie)).ReturnsAsync(updatedMovie);

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.PutMovie(1, userMovie) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);

            var objectResult = (MovieDTO)result.Value;

            Assert.Equal(updatedMovie.Id, objectResult.Id);
            Assert.Equal(updatedMovie.Title, objectResult.Title);
            Assert.Equal(updatedMovie.Description, objectResult.Description);
            Assert.Equal(updatedMovie.Duration, objectResult.Duration);
            Assert.Equal(updatedMovie.ReleaseDate, objectResult.ReleaseDate);
            Assert.Equal(updatedMovie.DirectorName, objectResult.DirectorName);
            Assert.Equal(updatedMovie.GenreName, objectResult.GenreName);
        }

        [Fact]
        public async Task PutMovie_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            Movie userMovie = new Movie
            {
                Id = 1,
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Duration = 153,
                ReleaseDate = new DateTime(1994, 10, 14),
                DirectorId = 1,
                GenreId = 6
            };

            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockService = new Mock<IMovieService>();

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.PutMovie(2, userMovie) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task PutMovie_InavlidId_ReturnsNotFound()
        {
            // Arrange
            Movie userMovie = new Movie
            {
                Id = 1,
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Duration = 153,
                ReleaseDate = new DateTime(1994, 10, 14),
                DirectorId = 1,
                GenreId = 6
            };

            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockService = new Mock<IMovieService>();

            var moviesController = new MoviesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await moviesController.PutMovie(1, userMovie) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}

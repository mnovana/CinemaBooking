using FilmService.Controllers;
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
    }
}

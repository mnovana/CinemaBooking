using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using FilmService.Models;
using FilmService.Models.DTO;
using SharedLibrary.Models.DTO;
using FilmService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FilmService.Services.Interfaces;

namespace FilmService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie with ID={id} not found.", id);
                return NotFound(new ProblemDetails 
                { 
                    Status = 404, 
                    Title = "Movie not found", 
                    Detail = $"The movie with ID={id} was not found in the database." 
                });
            }
            else
            {
                return Ok(movie);
            }
        }

        [AllowAnonymous]
        [HttpGet("titles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMovieTitlesByIds([FromQuery]string ids)
        {
            int[] idsArray = ids
                .Split(',')
                .Select(stringId => int.Parse(stringId))
                .ToArray();
            
            return Ok(await _movieService.GetTitlesByIdsAsync(idsArray));

        }

        [AllowAnonymous]
        [HttpGet("titleduration/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovieTitleDuration(int id)
        {
            MovieTitleDurationDTO? movie = await _movieService.GetTitleDurationByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Movie not found",
                    Detail = $"The movie with ID={id} was not found in the database."
                });
            }
            else
            {
                return Ok(movie);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovies()
        {
            return Ok(await _movieService.GetAllAsync());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostMovie(Movie movie)
        {
            var createdMovie = await _movieService.AddAsync(movie);

            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, createdMovie);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                _logger.LogWarning("Parameter ID and movie ID do not match.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid ID",
                    Detail = $"The movie could not be updated because the ID does not match the ID sent as a parameter."
                });
            }

            var updatedMovie = await _movieService.UpdateAsync(movie);
            if (updatedMovie == null)
            {
                _logger.LogWarning("Movie with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Movie not found",
                    Detail = $"The movie with ID={id} could not be updated because it was not found in the database."
                });
            }

            return Ok(updatedMovie);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (await _movieService.DeleteAsync(id))
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Movie with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Movie not found",
                    Detail = $"The movie with ID={id} could not be deleted because it was not found in the database."
                });
            }
        }
    }
}

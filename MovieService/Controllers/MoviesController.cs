using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using MovieService.Models;
using MovieService.Models.DTO;
using MovieService.Repositories.Interfaces;

namespace MovieService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<MoviesController> _logger;
        private readonly IMapper _mapper;

        public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

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
                return Ok(_mapper.Map<MovieDTO>(movie));
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMovieTitlesByIds([FromQuery]List<int> ids)
        {
            return Ok(await _movieRepository.GetTitlesByIdsAsync(ids));

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieRepository.GetAllAsync();
            
            return Ok(movies.AsQueryable().ProjectTo<MovieDTO>(_mapper.ConfigurationProvider));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Movie object is invalid.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid movie",
                    Detail = "New movie could not be added to the database because the model state is not valid."
                });
            }

            await _movieRepository.AddAsync(movie);

            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, _mapper.Map<MovieDTO>(movie));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Movie object is invalid.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid movie",
                    Detail = "The movie could not be updated because the model state is not valid."
                });
            }

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

            if (await _movieRepository.UpdateAsync(movie))
            {
                return Ok(_mapper.Map<MovieDTO>(movie));
            }
            else
            {
                _logger.LogWarning("Movie with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Movie not found",
                    Detail = $"The movie with ID={id} could not be updated because it was not found in the database."
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (await _movieRepository.DeleteAsync(id))
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

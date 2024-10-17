using Microsoft.AspNetCore.Mvc;
using MovieService.Models;
using MovieService.Repositories.Interfaces;

namespace MovieService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly IDirectorRepository _directorRepository;
        private readonly ILogger<DirectorsController> _logger;

        public DirectorsController(IDirectorRepository directorRepository, ILogger<DirectorsController> logger)
        {
            _directorRepository = directorRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDirectorById(int id)
        {
            var director = await _directorRepository.GetByIdAsync(id);

            if (director == null)
            {
                _logger.LogWarning("Director with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Director not found",
                    Detail = $"The director with ID={id} was not found in the database."
                });
            }
            else
            {
                return Ok(director);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDirectors()
        {
            return Ok(await _directorRepository.GetAllAsync());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostDirector(Director director)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Director object is invalid.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid director",
                    Detail = "New director could not be added to the database because the model state is not valid."
                });
            }

            await _directorRepository.AddAsync(director);

            return CreatedAtAction(nameof(GetDirectorById), new { id = director.Id }, director);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDirector(int id, Director director)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Director object is invalid.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid director",
                    Detail = "The director could not be updated because the model state is not valid."
                });
            }

            if (id != director.Id)
            {
                _logger.LogWarning("Parameter ID and director ID do not match.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid ID",
                    Detail = "The director could not be updated because the ID does not match the ID sent as a parameter."
                });
            }

            if (await _directorRepository.UpdateAsync(director))
            {
                return Ok(director);
            }
            else
            {
                _logger.LogWarning("Director with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Director not found",
                    Detail = $"The director with ID={id} could not be updated because it was not found in the database."
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            if (await _directorRepository.DeleteAsync(id))
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Director with ID={id} not found.", id);
                return BadRequest(new ProblemDetails
                {
                    Status = 404,
                    Title = "Director not found",
                    Detail = $"The director with ID={id} could not be deleted because it was not found in the database."
                });
            }
        }
    }
}

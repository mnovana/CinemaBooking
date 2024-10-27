using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScreeningService.Models;
using ScreeningService.Services.Interfaces;

namespace ScreeningService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<ShowtimesController> _logger;

        public ShowtimesController(IShowtimeService showtimeService, ILogger<ShowtimesController> logger)
        {
            _showtimeService = showtimeService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowtimeById(int id)
        {
            var showtime = await _showtimeService.GetByIdAsync(id);

            if (showtime == null)
            {
                _logger.LogWarning("Showtime with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Showtime not found",
                    Detail = $"The showtime with ID={id} was not found in the database."
                });
            }

            return Ok(showtime);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowtimes()
        {
            var showtimes = await _showtimeService.GetAllAsync();

            return Ok(showtimes);
        }

        [HttpGet("ByIds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowtimesByIds(string ids)
        {
            int[] idsArray = ids
                 .Split(',')
                 .Select(stringId => int.Parse(stringId))
                 .ToArray();

            var showtimes = await _showtimeService.GetAllAsync();

            return Ok(showtimes);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostShowtime(Showtime showtime)
        {
            var createdShowtime = await _showtimeService.AddAsync(showtime);
            
            return CreatedAtAction(nameof(GetShowtimeById), new { id = createdShowtime.Id}, createdShowtime);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutShowtime(int id, Showtime showtime)
        {
            if (id != showtime.Id)
            {
                _logger.LogWarning("Parameter ID and showtime ID do not match.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid ID",
                    Detail = $"The showtime could not be updated because the ID does not match the ID sent as a parameter."
                });
            }

            var updatedShowtime = await _showtimeService.UpdateAsync(showtime);

            if (updatedShowtime == null)
            {
                _logger.LogWarning("Showtime with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Showtime not found",
                    Detail = $"The showtime with ID={id} could not be updated because it was not found in the database."
                });
            }

            return Ok(updatedShowtime);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            if (await _showtimeService.DeleteAsync(id))
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Showtime with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Showtime not found",
                    Detail = $"The showtime with ID={id} could not be deleted because it was not found in the database."
                });
            }
        }

    }
}

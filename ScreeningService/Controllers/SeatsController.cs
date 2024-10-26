using Microsoft.AspNetCore.Mvc;
using ScreeningService.Services.Interfaces;

namespace ScreeningService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly ILogger<SeatsController> _logger;

        public SeatsController(ISeatService seatService, ILogger<SeatsController> logger)
        {
            _seatService = seatService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSeats()
        {
            return Ok(await _seatService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSeatById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
            {
                _logger.LogWarning("Seat with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Seat not found",
                    Detail = $"The seat with ID={id} was not found in the database."
                });
            }

            return Ok(seat);
        }

        [HttpGet("ByIds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSeatsByIds([FromQuery] string ids)
        {
            int[] idsArray = ids
                .Split(',')
                .Select(stringId => int.Parse(stringId))
                .ToArray();

            return Ok(await _seatService.GetByIdsAsync(idsArray));
        }

        [HttpGet("CheckRoom")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // get seats only if they all belong to the passed screening room
        // to prevent booking a showtime with a seat in a wrong screening room
        public async Task<IActionResult> GetSeatsByIdsIfRoomMatches([FromQuery] string ids, int screeningRoomNumber)
        {
            int[] idsArray = ids
                .Split(',')
                .Select(stringId => int.Parse(stringId))
                .ToArray();

            return Ok(await _seatService.GetByIdsIfRoomMatchesAsync(idsArray, screeningRoomNumber));
        }

    }
}

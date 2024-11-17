using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Models;
using SeatReservationService.Services.Interfaces;

namespace SeatReservationService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Reservation not found",
                    Detail = $"The reservation with ID={id} was not found in the database."
                });
            }

            return Ok(reservation);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _reservationService.GetAllAsync();

            return Ok(reservations);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostReservation(Reservation reservation)
        {
            var createdReservation = await _reservationService.AddAsync(reservation);

            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.Id }, createdReservation);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                _logger.LogWarning("Parameter ID and reservation ID do not match.");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid ID",
                    Detail = $"The reservation could not be updated because the ID does not match the ID sent as a parameter."
                });
            }

            var updatedReservation = await _reservationService.UpdateAsync(reservation);

            if (updatedReservation == null)
            {
                _logger.LogWarning("Reservation with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Reservation not found",
                    Detail = $"The reservation with ID={id} could not be updated because it was not found in the database."
                });
            }

            return Ok(updatedReservation);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            if (await _reservationService.DeleteAsync(id))
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Reservation with ID={id} not found.", id);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Reservation not found",
                    Detail = $"The reservation with ID={id} could not be deleted because it was not found in the database."
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("showtime/{showtimeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReservationWithShowtimeExists(int showtimeId)
        {
            if(!await _reservationService.ReservationWithShowtimeIdExistsAsync(showtimeId))
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Reservations do not exist",
                    Detail = $"There are no reservations with showtimeID={showtimeId}."
                });
            }

            return Ok();
        }
    }
}

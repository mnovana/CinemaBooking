using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<List<ShowtimeDTO>> GetShowtimesAsync();
    }
}

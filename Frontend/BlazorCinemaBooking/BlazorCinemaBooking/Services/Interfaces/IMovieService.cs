using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieDTO> GetMovieById(int id);
    }
}

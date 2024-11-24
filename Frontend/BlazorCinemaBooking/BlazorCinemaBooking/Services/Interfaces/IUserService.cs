using BlazorCinemaBooking.Models;
using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services.Interfaces
{
    public interface IUserService
    {
        Task LoginAsync(Login model);
        void Logout();
    }
}

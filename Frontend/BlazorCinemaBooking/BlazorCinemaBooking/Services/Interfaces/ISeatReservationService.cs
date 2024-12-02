namespace BlazorCinemaBooking.Services.Interfaces
{
    public interface ISeatReservationService
    {
        Task AddReservationAsync(int showtimeId, string? userEmail, int[] seats);
    }
}

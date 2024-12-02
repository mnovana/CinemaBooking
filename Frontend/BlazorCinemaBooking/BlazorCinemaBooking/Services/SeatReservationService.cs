using BlazorCinemaBooking.Services.Interfaces;

namespace BlazorCinemaBooking.Services
{
    public class SeatReservationService : ISeatReservationService
    {
        private readonly HttpClient _httpClient;

        public SeatReservationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5002");
        }

        public async Task AddReservationAsync(int showtimeId, string? userEmail, int[] seats)
        {
            var response = await _httpClient.PostAsJsonAsync("gateway/reservations", new { 
                ShowtimeId = showtimeId, 
                UserEmail = userEmail, 
                ReservedSeats = seats 
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to add reservation, status code {response.StatusCode}.");
            }
        }
    }
}

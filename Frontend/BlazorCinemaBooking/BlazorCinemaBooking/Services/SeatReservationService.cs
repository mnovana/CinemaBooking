using BlazorCinemaBooking.Services.Interfaces;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace BlazorCinemaBooking.Services
{
    public class SeatReservationService : ISeatReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public SeatReservationService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5002");
            _localStorageService = localStorageService;
        }

        public async Task AddReservationAsync(int showtimeId, string userEmail, int[] seats)
        {
            var token = await _localStorageService.GetItemAsStringAsync("jwt_token");
            token = token.Trim('"');
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsJsonAsync("gateway/reservations", new { 
                ShowtimeId = showtimeId, 
                UserEmail = userEmail, 
                ReservedSeats = seats.Select(seat => new { seatId = seat }).ToArray() 
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to add reservation, status code {response.StatusCode}.");
            }
        }
    }
}

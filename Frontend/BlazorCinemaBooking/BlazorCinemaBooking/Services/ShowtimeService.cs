using BlazorCinemaBooking.Services.Interfaces;
using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly HttpClient _httpClient;

        public ShowtimeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<List<ShowtimeDTO>> GetShowtimesAsync()
        {
            //var response = await _httpClient.GetAsync("");

            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception($"Failed to fetch showtimes, status code {response.StatusCode}.");
            //}

            //var fetchedShowtimes = await response.Content.ReadFromJsonAsync<List<ShowtimeDTO>>() ?? [];

            //return fetchedShowtimes;
            await Task.Delay(2000);

            return new List<ShowtimeDTO>
            {
                new ShowtimeDTO { Id = 1, MovieId = 1, MovieTitle = "Movie One", ScreeningRoomNumber = 1, Start = new DateTime(2024,11,21,18,0,0) },
                new ShowtimeDTO { Id = 2, MovieId = 1, MovieTitle = "Movie One", ScreeningRoomNumber = 2, Start = new DateTime(2024,11,21,20,30,0) },
                new ShowtimeDTO { Id = 3, MovieId = 2, MovieTitle = "Movie Two", ScreeningRoomNumber = 1, Start = new DateTime(2024,11,22,17,0,0) },
                new ShowtimeDTO { Id = 4, MovieId = 2, MovieTitle = "Movie Two", ScreeningRoomNumber = 3, Start = new DateTime(2024,11,22,18,45,0) },
            };
        }
    }
}

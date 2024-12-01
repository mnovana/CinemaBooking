using BlazorCinemaBooking.Services.Interfaces;
using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5002");
        }

        public async Task<MovieDTO> GetMovieById(int id)
        {
            //var response = await _httpClient.GetAsync($"{id}");

            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception($"Failed to fetch movie with ID={id}, status code {response.StatusCode}.");
            //}

            //var fetchedMovie = await response.Content.ReadFromJsonAsync<MovieDTO>();

            //return fetchedMovie;

            await Task.Delay(2000);
            return new MovieDTO
            {
                Id = id,
                Title = $"Movie {id}",
                Description = "Des c ription s fnew  of  eiweugowewef f wefe wf wfgr gajoioweh. HEFELKFEjajfwefjioifkle  wefjieiof regj oeifj.",
                Duration = 120,
                ReleaseDate = new DateTime(2015,12,3),
                DirectorName = "Novana Maraavic",
                GenreName = "Action"
            };
        }
    }
}

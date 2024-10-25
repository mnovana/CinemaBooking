using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScreeningService.Models;
using ScreeningService.Models.DTO;
using SharedLibrary.Models.DTO;
using ScreeningService.Repositories.Interfaces;
using ScreeningService.Services.Interfaces;

namespace ScreeningService.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ShowtimeService(IShowtimeRepository showtimeRepository, HttpClient httpClient, IMapper mapper)
        {
            _showtimeRepository = showtimeRepository;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("MOVIE_SERVICE_URL"));
            _mapper = mapper;
        }

        public async Task<ShowtimeDTO> AddAsync(Showtime showtime)
        {
            var response = await _httpClient.GetAsync($"movies/titleduration/{showtime.MovieId}");

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movie title with ID={showtime.MovieId}, status code {response.StatusCode}.");
            }
            
            var movieDto = await response.Content.ReadFromJsonAsync<MovieTitleDurationDTO>();

            // calculate end and check overlapping
            showtime.End = showtime.Start.AddMinutes(movieDto.Duration);

            if (await _showtimeRepository.ShowtimesOverlap(showtime))
            {
                throw new Exception($"Bad request, showtimes are overlapping.");
            }

            // set referenced data to null to prevent inserting a new row to a referenced table
            showtime.ScreeningRoom = null;

            // add showtime and fetch it for referenced data
            await _showtimeRepository.AddAsync(showtime);
            var addedShowtime = await _showtimeRepository.GetByIdAsync(showtime.Id);
            var showtimeDto = _mapper.Map<ShowtimeDTO>(addedShowtime);

            // set missing data fetched from another db
            showtimeDto.MovieTitle = movieDto.Title;

            return showtimeDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _showtimeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShowtimeDTO>> GetAllAsync()
        {
            // get showtimes and extract movie ids needed for a request
            var showtimes = await _showtimeRepository.GetAllAsync();
            var movieIds = showtimes.Select(s => s.MovieId).Distinct();

            var response = await _httpClient.GetAsync($"movies/titles?ids={string.Join(",", movieIds)}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movie titles, status code {response.StatusCode}.");
            }

            var movieTitles = await response.Content.ReadFromJsonAsync<List<MovieTitleDTO>>();
            var showtimesDto = showtimes.AsQueryable().ProjectTo<ShowtimeDTO>(_mapper.ConfigurationProvider).ToList();

            // set showtimes movie titles using a dictionary
            var movieTitlesDict = movieTitles.ToDictionary(m => m.Id, m => m.Title);
            foreach(var showtime in showtimesDto)
            {
                if (movieTitlesDict.TryGetValue(showtime.MovieId, out var title))
                {
                    showtime.MovieTitle = title;
                }
                else
                {
                    showtime.MovieTitle = "unknown";
                }
            }

            return showtimesDto;
        }

        public async Task<ShowtimeDTO?> GetById(int id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);

            if (showtime == null)
            {
                return null;
            }

            var response = await _httpClient.GetAsync($"movies/titles?ids={showtime.MovieId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movie title with ID={showtime.MovieId}, status code {response.StatusCode}.");
            }

            var movieTitles = await response.Content.ReadFromJsonAsync<List<MovieTitleDTO>>();
            var showtimeDto = _mapper.Map<ShowtimeDTO>(showtime);
            showtimeDto.MovieTitle = movieTitles.First().Title;

            return showtimeDto;
        }

        public async Task<ShowtimeDTO?> UpdateAsync(Showtime showtime)
        {
            var response = await _httpClient.GetAsync($"movies/titles?ids={showtime.MovieId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movie title with ID={showtime.MovieId}, status code {response.StatusCode}.");
            }

            var movieTitles = await response.Content.ReadFromJsonAsync<List<MovieTitleDTO>>();

            if (movieTitles.IsNullOrEmpty())
            {
                throw new Exception($"Bad request, movie with ID={showtime.MovieId} does not exist.");
            }

            if (!await _showtimeRepository.UpdateAsync(showtime))
            {
                return null;
            }

            var showtimeDto = _mapper.Map<ShowtimeDTO>(showtime);
            showtimeDto.MovieTitle = movieTitles.First().Title;

            return showtimeDto;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.IdentityModel.Tokens;
using ScreeningService.Models;
using SharedLibrary.Models.DTO;
using ScreeningService.Repositories.Interfaces;
using ScreeningService.Services.Interfaces;
using SharedLibrary.Services.Interfaces;
using System.Net.Http.Headers;

namespace ScreeningService.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShowtimeService(IShowtimeRepository showtimeRepository, IHttpClientFactory httpClientFactory, IMapper mapper, ICacheService cacheService, IHttpContextAccessor httpContextAccessor )
        {
            _showtimeRepository = showtimeRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ShowtimeDTO> AddAsync(Showtime showtime)
        {
            // fetch title and duration from cache or FilmService
            var movieTitleDurationDto = await GetMovieTitleDurationAsync(showtime.MovieId);

            // calculate end and check overlapping
            showtime.End = showtime.Start.AddMinutes(movieTitleDurationDto.Duration);

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

            // set missing data fetched earlier from another db
            showtimeDto.MovieTitle = movieTitleDurationDto.Title;

            return showtimeDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (await ReservationWithShowtimeExistsAsync(id))
            {
                throw new Exception($"Bad request, showtime with ID={id} could not be deleted because at least one reservation uses it.");
            }

            await _cacheService.RemoveDataAsync($"showtime-{id}");
            
            return await _showtimeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShowtimeDTO>> GetAllAsync()
        {
            // get showtimes and extract movie ids needed for a request
            var showtimes = await _showtimeRepository.GetAllAsync();
            var movieIds = showtimes.Select(s => s.MovieId).Distinct().ToArray();

            // fetch movie titles from FilmService
            var movieTitles = await GetMovieTitlesByIdsAsync(movieIds);

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

        public async Task<ShowtimeDTO?> GetByIdAsync(int id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);

            if (showtime == null)
            {
                return null;
            }

            var movieTitles = await GetMovieTitlesByIdsAsync( [showtime.MovieId] );
            var showtimeDto = _mapper.Map<ShowtimeDTO>(showtime);
            showtimeDto.MovieTitle = movieTitles.IsNullOrEmpty() ? "unknown" : movieTitles.First().Title;

            return showtimeDto;
        }

        public async Task<IEnumerable<ShowtimeDTO>> GetByIdsAsync(int[] ids)
        {
            // get showtimes and extract movie ids needed for a request
            var showtimes = await _showtimeRepository.GetByIdsAsync(ids);
            var movieIds = showtimes.Select(s => s.MovieId).Distinct().ToArray();

            // fetch movie titles from FilmService
            var movieTitles = await GetMovieTitlesByIdsAsync(movieIds);

            var showtimesDto = showtimes.AsQueryable().ProjectTo<ShowtimeDTO>(_mapper.ConfigurationProvider).ToList();

            // set showtimes movie titles using a dictionary
            var movieTitlesDict = movieTitles.ToDictionary(m => m.Id, m => m.Title);
            foreach (var showtime in showtimesDto)
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

        public async Task<bool> ShowtimeWithMovieIdExistsAsync(int movieId)
        {
            return await _showtimeRepository.ShowtimeWithMovieIdExistsAsync(movieId);
        }

        public async Task<ShowtimeDTO?> UpdateAsync(Showtime showtime)
        {
            // fetch title and duration from cache or FilmService
            var movieTitleDurationDto = await GetMovieTitleDurationAsync(showtime.MovieId);

            // calculate end and check overlapping
            showtime.End = showtime.Start.AddMinutes(movieTitleDurationDto.Duration);

            if (await _showtimeRepository.ShowtimesOverlap(showtime))
            {
                throw new Exception($"Bad request, showtimes are overlapping.");
            }

            // update showtime and fetch it for referenced data
            if (!await _showtimeRepository.UpdateAsync(showtime))
            {
                return null;
            }

            var updatedShowtime = await _showtimeRepository.GetByIdAsync(showtime.Id);
            var showtimeDto = _mapper.Map<ShowtimeDTO>(updatedShowtime);

            // set missing data fetched earlier from another db
            showtimeDto.MovieTitle = movieTitleDurationDto.Title;

            return showtimeDto;
        }

        private async Task<MovieTitleDurationDTO> GetMovieTitleDurationAsync(int id)
        {
            // search movie in cache
            var cachedMovieTitleDuration = await _cacheService.GetDataAsync<MovieTitleDurationDTO>($"movieTitleDuration-{id}");

            // if not existent, fetch title and duration from FilmService
            if (cachedMovieTitleDuration != null)
            {
                return cachedMovieTitleDuration;
            }
            else
            {
                var client = _httpClientFactory.CreateClient("FilmService");
                var response = await client.GetAsync($"movies/titleduration/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch movie title with ID={id}, status code {response.StatusCode}.");
                }

                var content = await response.Content.ReadFromJsonAsync<MovieTitleDurationDTO>();

                if (content == null)
                {
                    throw new Exception($"Failed to read the response from {response.RequestMessage.RequestUri.ToString()}.");
                }

                // set cache
                await _cacheService.SetDataAsync($"movieTitleDuration-{id}", content, TimeSpan.FromHours(1));
                
                return content;
            }
        }

        private async Task<List<MovieTitleDTO>> GetMovieTitlesByIdsAsync(int[] ids)
        {
            // search for titles in cache
            var cachedMovieTitles = await GetCachedMovieTitlesByIdsAsync(ids);

            // find missing movie ids and delete nulls from the list of movies
            // use the fact that ids and cachedMoviesTitles have the same order
            List<int> missingIds = new List<int>();
            for (int i=cachedMovieTitles.Count-1; i>=0; i--)
            {
                if (cachedMovieTitles[i] == null)
                {
                    missingIds.Add(ids[i]);
                    cachedMovieTitles.RemoveAt(i);
                }
            }

            if (missingIds.IsNullOrEmpty())
            {
                return cachedMovieTitles;
            }
            else
            {
                var client = _httpClientFactory.CreateClient("FilmService");
                var response = await client.GetAsync($"movies/titles?ids={string.Join(",", missingIds)}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch movie titles, status code {response.StatusCode}.");
                }

                var fetchedMovieTitles = await response.Content.ReadFromJsonAsync<List<MovieTitleDTO>>() ?? [];

                // set cache
                var fetchedMovieTitlesDict = fetchedMovieTitles.ToDictionary(m => $"movieTitle-{m.Id}", m => m);
                await _cacheService.SetMultipleDataAsync(fetchedMovieTitlesDict, TimeSpan.FromHours(1));

                // combine fetched and cached titles
                fetchedMovieTitles.AddRange(cachedMovieTitles);

                return fetchedMovieTitles;
            }
        }

        private async Task<List<MovieTitleDTO?>> GetCachedMovieTitlesByIdsAsync(int[] ids)
        {
            string[] keys = ids.Select(i => $"movieTitle-{i}").ToArray();
            var cachedMovieTitles = await _cacheService.GetMultipleDataAsync<MovieTitleDTO>(keys);

            return cachedMovieTitles.ToList();
        }

        private async Task<bool> ReservationWithShowtimeExistsAsync(int showtimeId)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization
                .ToString()
                .Replace("Bearer ", "");

            var client = _httpClientFactory.CreateClient("SeatReservationService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"reservations/showtime/{showtimeId}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if ((int)response.StatusCode == StatusCodes.Status404NotFound)
            {
                return false;
            }
            else
            {
                throw new Exception($"Failed to check if reservation exists, response from SeatReservationService: {response.StatusCode}");
            }
        }
    }
}

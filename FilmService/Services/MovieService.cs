using AutoMapper;
using AutoMapper.QueryableExtensions;
using FilmService.Models;
using FilmService.Repositories.Interfaces;
using FilmService.Services.Interfaces;
using SharedLibrary.Models.DTO;
using SharedLibrary.Services.Interfaces;

namespace FilmService.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;

        public MovieService(IMovieRepository movieRepository, IMapper mapper, HttpClient httpClient, ICacheService cacheService)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _cacheService = cacheService;
        }
        
        public async Task<MovieDTO> AddAsync(Movie movie)
        {
            await _movieRepository.AddAsync(movie);

            // return fetched movie with referenced data
            return _mapper.Map<MovieDTO>(await _movieRepository.GetByIdAsync(movie.Id));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (await ShowtimeWithMovieExistsAsync(id))
            {
                throw new Exception($"Bad request, movie with ID={id} could not be deleted because at least one showtime uses it.");
            }

            return await _movieRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MovieDTO>> GetAllAsync()
        {
            var movies = await _movieRepository.GetAllAsync();

            return movies.AsQueryable().ProjectTo<MovieDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<MovieDTO?> GetByIdAsync(int id)
        {
            return _mapper.Map<MovieDTO>(await _movieRepository.GetByIdAsync(id));
        }

        public async Task<MovieTitleDurationDTO?> GetTitleDurationByIdAsync(int id)
        {
            return await _movieRepository.GetTitleDurationByIdAsync(id);
        }

        public async Task<IEnumerable<MovieTitleDTO>> GetTitlesByIdsAsync(int[] ids)
        {
            return await _movieRepository.GetTitlesByIdsAsync(ids);
        }

        public async Task<MovieDTO?> UpdateAsync(Movie movie)
        {
            if (!await _movieRepository.UpdateAsync(movie))
            {
                return null;
            }

            // return fetched movie with referenced data
            return _mapper.Map<MovieDTO>(await _movieRepository.GetByIdAsync(movie.Id));
        }

        private async Task<bool> ShowtimeWithMovieExistsAsync(int movieId)
        {
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SCREENING_SERVICE_URL"));
            var response = await _httpClient.GetAsync($"showtimes/movie/{movieId}");

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
                throw new Exception($"Failed to check if showtime with MovieID={movieId} exists, response from ScreeningService: {response.StatusCode}");
            }
        }
    }
}

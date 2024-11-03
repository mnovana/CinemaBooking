﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.IdentityModel.Tokens;
using ScreeningService.Models;
using SharedLibrary.Models.DTO;
using ScreeningService.Repositories.Interfaces;
using ScreeningService.Services.Interfaces;

namespace ScreeningService.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;

        public ShowtimeService(IShowtimeRepository showtimeRepository, IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _showtimeRepository = showtimeRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ShowtimeDTO> AddAsync(Showtime showtime)
        {   
            // fetch title and duration from FilmService
            var movieDto = await GetMovieTitleDuration(showtime.MovieId);

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

            // set missing data fetched earlier from another db
            showtimeDto.MovieTitle = movieDto.Title;

            return showtimeDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (await ReservationWithShowtimeExistsAsync(id))
            {
                throw new Exception($"Bad request, showtime with ID={id} could not be deleted because at least one reservation uses it.");
            }
            
            return await _showtimeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShowtimeDTO>> GetAllAsync()
        {
            // get showtimes and extract movie ids needed for a request
            var showtimes = await _showtimeRepository.GetAllAsync();
            var movieIds = showtimes.Select(s => s.MovieId).Distinct().ToArray();

            // fetch movie titles from FilmService
            var movieTitles = await GetMovieTitlesByIds(movieIds);

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

            var movieTitles = await GetMovieTitlesByIds( [showtime.MovieId] );
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
            var movieTitles = await GetMovieTitlesByIds(movieIds);

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

        public async Task<ShowtimeDTO?> UpdateAsync(Showtime showtime)
        {
            // fetch title and duration from FilmService
            var movieDto = await GetMovieTitleDuration(showtime.MovieId);

            // calculate end and check overlapping
            showtime.End = showtime.Start.AddMinutes(movieDto.Duration);

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
            showtimeDto.MovieTitle = movieDto.Title;

            return showtimeDto;
        }

        private async Task<MovieTitleDurationDTO> GetMovieTitleDuration(int id)
        {
            var client = _httpClientFactory.CreateClient("FilmService");
            var response = await client.GetAsync($"movies/titleduration/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movie title with ID={id}, status code {response.StatusCode}.");
            }

            var content = await response.Content.ReadFromJsonAsync<MovieTitleDurationDTO>();

            return content ?? throw new Exception($"Failed to read the response from {response.RequestMessage.RequestUri.ToString()}.");
        }

        private async Task<List<MovieTitleDTO>> GetMovieTitlesByIds(int[] ids)
        {
            var client = _httpClientFactory.CreateClient("FilmService");
            var response = await client.GetAsync($"movies/titles?ids={string.Join(",", ids)}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movie titles, status code {response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<List<MovieTitleDTO>>() ?? [];
        }

        private async Task<bool> ReservationWithShowtimeExistsAsync(int showtimeId)
        {
            var client = _httpClientFactory.CreateClient("SeatReservationService");
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

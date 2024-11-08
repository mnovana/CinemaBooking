using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Models;
using SeatReservationService.Models.DTO;
using SeatReservationService.Repositories.Interfaces;
using SeatReservationService.Services.Interfaces;
using SharedLibrary.Models.DTO;
using System.Security.Claims;
using System.Net.Http.Headers;
using SharedLibrary.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace SeatReservationService.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly string _token;

        public ReservationService(IReservationRepository reservationRepository, IHttpClientFactory httpClientFactory, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICacheService cacheService)
        {
            _reservationRepository = reservationRepository;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;

            _token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");
        }

        public async Task<ReservationDTO> AddAsync(Reservation reservation)
        {
            // get showtime if existent
            var showtime = await GetShowtimeByIdAsync(reservation.ShowtimeId);

            // get seats if existent and available
            var seatIds = reservation.ReservedSeats.Select(rs => rs.SeatId).ToArray();

            var takenSeats = await _reservationRepository.GetTakenSeatsAsync(showtime.Id, seatIds);
            if(takenSeats.Count() > 0)
            {
                throw new Exception($"Bad request, seats with these IDs are already reserved: {string.Join(',', takenSeats)}.");
            }
            var seats = await GetSeatsByIdsAsync(seatIds, showtime.ScreeningRoomNumber);

            // extract role from jwt token, if it's:
            // admin - check if email exists in UserService
            // user - set reservation email to that user's email
            var roles = _httpContextAccessor.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if(roles.Contains("Admin"))
            {
                if (!await EmailExistsAsync(reservation.UserEmail))
                {
                    throw new Exception($"Bad request, email '{reservation.UserEmail}' does not exist in the database.");
                }
            }
            else if(roles.Contains("User"))
            {
                reservation.UserEmail = _httpContextAccessor.HttpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.Email)
                    .Select(c => c.Value)
                    .Single();
            }
            else
            {
                throw new Exception("Bad request, invalid user role.");
            }

            // add
            await _reservationRepository.AddAsync(reservation);

            // fetch for referenced properties
            ReservationDTO createdReservation = _mapper.Map<ReservationDTO>(await _reservationRepository.GetByIdAsync(reservation.Id));
            
            // set showtime and reserved seats
            createdReservation.Showtime = showtime;
            createdReservation.ReservedSeats = seats
                .Select(s => new ReservedSeatDTO
                {
                    SeatNumber = s.Number,
                    SeatRowNumber = s.RowNumber
                }).ToList();
            
            return createdReservation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _reservationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ReservationDTO>> GetAllAsync()
        {
            // get reservations and extract showtimes and seats ids needed for fetching
            var reservations = await _reservationRepository.GetAllAsync();
            var showtimeIds = reservations.Select(r => r.ShowtimeId).Distinct().ToArray();
            var seatIds = reservations.SelectMany(r => r.ReservedSeats.Select(rs => rs.SeatId)).Distinct().ToArray();

            // fetch
            var showtimes = await GetShowtimesByIdsAsync(showtimeIds);
            var seats = await GetSeatsByIdsAsync(seatIds);

            // set showtimes using a dictionary
            var showtimesDict = showtimes.ToDictionary(s => s.Id, s => s);
            foreach (var reservation in reservations)
            {
                if (showtimesDict.TryGetValue(reservation.ShowtimeId, out var showtime))
                {
                    reservation.Showtime = showtime;
                }
            }

            // set seats using a dictionary
            var seatsDict = seats.ToDictionary(s => s.Id, s => s);
            foreach (var reservation in reservations)
            {
                foreach (var reservedSeat in reservation.ReservedSeats)
                {
                    if (seatsDict.TryGetValue(reservedSeat.SeatId, out var seat))
                    {
                        reservedSeat.Seat = seat;
                    }
                }
            }

            return reservations.AsQueryable().ProjectTo<ReservationDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<ReservationDTO?> GetByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                return null;
            }

            // fetch showtime
            reservation.Showtime = await GetShowtimeByIdAsync(reservation.ShowtimeId);
            
            //fetch seats
            var seatIds = reservation.ReservedSeats.Select(rs => rs.SeatId).ToArray();
            var seats = await GetSeatsByIdsAsync(seatIds);

            // set seats using a dictionary
            var seatsDict = seats.ToDictionary(s => s.Id, s => s);
            foreach (var reservedSeat in reservation.ReservedSeats)
            {
                if (seatsDict.TryGetValue(reservedSeat.SeatId, out var seat))
                {
                    reservedSeat.Seat = seat;
                }
            }

            return _mapper.Map<ReservationDTO>(reservation);            
        }

        public async Task<ReservationDTO?> UpdateAsync(Reservation reservation)
        {
            // get showtime if existent
            var showtime = await GetShowtimeByIdAsync(reservation.ShowtimeId);

            // get seats if existent and available
            var seatIds = reservation.ReservedSeats.Select(rs => rs.SeatId).ToArray();

            var takenSeats = await _reservationRepository.GetTakenSeatsAsync(showtime.Id, seatIds);
            if (takenSeats.Count() > 0)
            {
                throw new Exception($"Bad request, seats with these IDs are already reserved: {string.Join(',', takenSeats)}.");
            }
            var seats = await GetSeatsByIdsAsync(seatIds, showtime.ScreeningRoomNumber);

            // check if email exists in UserService (no role checking since only admin can update)
            if (!await EmailExistsAsync(reservation.UserEmail))
            {
                throw new Exception($"Bad request, email '{reservation.UserEmail}' does not exist in the database.");
            }

            // update (false means ID not found)
            if (!await _reservationRepository.UpdateAsync(reservation))
            {
                return null;
            }

            // set referenced properties: showtime and reserved seats
            reservation.Showtime = showtime;
            reservation.ReservedSeats = seats
                .Select(s => new ReservedSeat { Seat = s })
                .ToList();

            return _mapper.Map<ReservationDTO>(reservation);
        }

        private async Task<ShowtimeDTO> GetShowtimeByIdAsync(int id)
        {
            // search for showtime in cache
            var cachedShowtime = await _cacheService.GetDataAsync<ShowtimeDTO>($"showtime-{id}");

            // if not existent, fetch from ScreeningService
            if (cachedShowtime != null)
            {
                return cachedShowtime;
            }
            else
            {
                var client = GetClientWithToken("ScreeningService");
                var response = await client.GetAsync($"showtimes/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch showtime with ID={id}, status code {response.StatusCode}.");
                }

                var content = await response.Content.ReadFromJsonAsync<ShowtimeDTO>();

                if (content == null)
                {
                    throw new Exception($"Failed to read the response from {response.RequestMessage.RequestUri.ToString()}.");
                }

                // set cache
                await _cacheService.SetDataAsync($"showtime-{id}", content, TimeSpan.FromHours(1));

                return content;
            }
        }

        private async Task<List<ShowtimeDTO>> GetShowtimesByIdsAsync(int[] ids)
        {
            // search for showtimes in cache
            var cachedShowtimes = await _cacheService.GetMultipleDataAsync<ShowtimeDTO>(ids.Select(id => $"showtime-{id}").ToArray());

            var idsHashset = new HashSet<int>(ids);
            var cachedIdsHashset = new HashSet<int>(cachedShowtimes.Select(showtime => showtime.Id));
            // missing ids
            idsHashset.ExceptWith(cachedIdsHashset);

            if (idsHashset.IsNullOrEmpty())
            {
                return cachedShowtimes.ToList();
            }
            else
            {
                var client = GetClientWithToken("ScreeningService");
                var response = await client.GetAsync($"showtimes/byids?ids={string.Join(',', idsHashset)}");

                if (!response.IsSuccessStatusCode)
                {
                    var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                    throw new Exception($"Failed to fetch showtimes: {problem.Detail}");
                }

                var fetchedShowtimes = await response.Content.ReadFromJsonAsync<List<ShowtimeDTO>>() ?? [];

                // set cache
                var fetchedShowtimesDict = fetchedShowtimes.ToDictionary(s => $"showtime-{s.Id}", s => s);
                await _cacheService.SetMultipleDataAsync(fetchedShowtimesDict, TimeSpan.FromHours(1));

                // combine fetched and cached showtimes
                fetchedShowtimes.AddRange(cachedShowtimes);

                return fetchedShowtimes;
            }
        }

        private async Task<List<SeatDTO>> GetSeatsByIdsAsync(int[] ids, int sreeningRoomNumber)
        {
            var client = GetClientWithToken("ScreeningService");
            var response = await client.GetAsync($"seats/checkroom?screeningroomnumber={sreeningRoomNumber}&ids={string.Join(",", ids)}");

            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                throw new Exception($"Failed to fetch seats: {problem.Detail}");
            }

            var seats = await response.Content.ReadFromJsonAsync<List<SeatDTO>>();

            return seats;
        }

        private async Task<List<SeatDTO>> GetSeatsByIdsAsync(int[] ids)
        {
            var client = GetClientWithToken("ScreeningService");
            var response = await client.GetAsync($"seats/byids?ids={string.Join(",", ids)}");

            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                throw new Exception($"Failed to fetch seats: {problem.Detail}");
            }

            var seats = await response.Content.ReadFromJsonAsync<List<SeatDTO>>();

            return seats;
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            var client = GetClientWithToken("UserService");
            var response = await client.GetAsync($"users/email?email={email}");

            if((int)response.StatusCode == 404)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Tried to check if email '{email}' exists, status code {response.StatusCode}.");
            }

            return true;
        }

        private HttpClient GetClientWithToken(string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            return client;
        }

        public async Task<bool> ReservationWithShowtimeIdExistsAsync(int showtimeId)
        {
            return await _reservationRepository.ReservationWithShowtimeIdExistsAsync(showtimeId);
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Models;
using SeatReservationService.Models.DTO;
using SeatReservationService.Repositories.Interfaces;
using SeatReservationService.Services.Interfaces;
using SharedLibrary.Models.DTO;
using System.Net.Http.Headers;

namespace SeatReservationService.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _token;

        public ReservationService(IReservationRepository reservationRepository, IHttpClientFactory httpClientFactory, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _reservationRepository = reservationRepository;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SCREENING_SERVICE_URL"));
        }

        public async Task<ReservationDTO> AddAsync(Reservation reservation)
        {
            // first check if showtime and seats exist
            var showtime = await GetShowtimeByIdAsync(reservation.ShowtimeId);

            var seatIds = reservation.ReservedSeats.Select(rs => rs.SeatId).ToArray();
            var seats = await GetSeatsByIdsAsync(seatIds, showtime.ScreeningRoomNumber);

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
            // first check if showtime and seats exist
            var showtime = await GetShowtimeByIdAsync(reservation.ShowtimeId);

            var seatIds = reservation.ReservedSeats.Select(rs => rs.SeatId).ToArray();
            var seats = await GetSeatsByIdsAsync(seatIds, showtime.ScreeningRoomNumber);

            // update (false means ID not found)
            if (!await _reservationRepository.UpdateAsync(reservation))
            {
                return null;
            }

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

        private async Task<ShowtimeDTO> GetShowtimeByIdAsync(int id)
        {
            var client = GetClientWithToken("ScreeningService");
            var response = await client.GetAsync($"showtimes/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch showtime with ID={id}, status code {response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<ShowtimeDTO>();
        }

        private async Task<List<ShowtimeDTO>> GetShowtimesByIdsAsync(int[] ids)
        {
            var client = GetClientWithToken("ScreeningService");
            var response = await client.GetAsync($"showtimes/byids?ids={string.Join(',', ids)}");

            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                throw new Exception($"Failed to fetch showtimes: {problem.Detail}");
            }

            return await response.Content.ReadFromJsonAsync<List<ShowtimeDTO>>();
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


        private HttpClient GetClientWithToken(string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            return client;
        }
    }
}

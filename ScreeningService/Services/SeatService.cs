using AutoMapper;
using AutoMapper.QueryableExtensions;
using ScreeningService.Repositories.Interfaces;
using ScreeningService.Services.Interfaces;
using SharedLibrary.Models.DTO;

namespace ScreeningService.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SeatService> _logger;
        
        public SeatService(ISeatRepository seatRepository, IMapper mapper, ILogger<SeatService> logger)
        {
            _seatRepository = seatRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<IEnumerable<SeatDTO>> GetAllAsync()
        {
            var seats = await _seatRepository.GetAllAsync();
            return seats.AsQueryable().ProjectTo<SeatDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<SeatDTO?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return null;
            }

            return _mapper.Map<SeatDTO>(seat);
        }

        public async Task<IEnumerable<SeatDTO>> GetByIdsAsync(int[] ids)
        {
            var seats = await _seatRepository.GetByIdsAsync(ids);

            return seats.AsQueryable().ProjectTo<SeatDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<IEnumerable<SeatDTO>> GetByIdsIfRoomMatchesAsync(int[] ids, int screeningRoomNumber)
        {
            var seats = await _seatRepository.GetByIdsAsync(ids);

            foreach (var seat in seats)
            {
                if (seat.Row.ScreeningRoom.Number != screeningRoomNumber)
                {
                    string errorMsg = $"Bad request, movie is showing in a screening room number {screeningRoomNumber} and you are trying to book a seat in a screening room number {seat.Row.ScreeningRoom.Number}";

                    _logger.LogWarning(errorMsg);

                    throw new Exception(errorMsg);
                }
            }

            return seats.AsQueryable().ProjectTo<SeatDTO>(_mapper.ConfigurationProvider);
        }

        
    }
}

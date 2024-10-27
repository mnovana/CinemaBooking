using AutoMapper;

namespace SeatReservationService.Models.DTO.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDTO>();
            CreateMap<ReservedSeat, ReservedSeatDTO>();
        }
    }
}

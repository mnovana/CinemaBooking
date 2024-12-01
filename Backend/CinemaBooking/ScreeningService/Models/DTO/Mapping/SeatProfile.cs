using AutoMapper;
using SharedLibrary.Models.DTO;

namespace ScreeningService.Models.DTO.Mapping
{
    public class SeatProfile : Profile
    {
        public SeatProfile()
        {
            CreateMap<Seat, SeatDTO>()
                .ForMember(
                    dest => dest.RowScreeningRoomNumber,
                    opt => opt.MapFrom(src => src.Row.ScreeningRoom.Number)
                );
        }
    }
}

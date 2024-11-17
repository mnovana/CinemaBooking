using AutoMapper;
using SharedLibrary.Models.DTO;

namespace ScreeningService.Models.DTO.Mapping
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile()
        {
            CreateMap<Showtime, ShowtimeDTO>()
                .ForMember(dest => dest.MovieTitle, opt => opt.Ignore());
        }
    }
}

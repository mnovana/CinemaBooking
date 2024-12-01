using AutoMapper;
using SharedLibrary.Models.DTO;

namespace FilmService.Models.DTO.Mapping
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieDTO>();
        }
    }
}

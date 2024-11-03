using AutoMapper;

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

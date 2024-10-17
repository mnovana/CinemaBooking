using AutoMapper;

namespace MovieService.Models.DTO.Mapping
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieDTO>();
        }
    }
}

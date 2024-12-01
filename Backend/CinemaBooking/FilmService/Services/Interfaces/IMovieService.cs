
using FilmService.Models;
using SharedLibrary.Models.DTO;

namespace FilmService.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDTO>> GetAllAsync();
        Task<MovieDTO?> GetByIdAsync(int id);
        Task<IEnumerable<MovieTitleDTO>> GetTitlesByIdsAsync(int[] ids);
        Task<MovieTitleDurationDTO?> GetTitleDurationByIdAsync(int id);
        Task<MovieDTO> AddAsync(Movie movie);
        Task<MovieDTO?> UpdateAsync(Movie movie);
        Task<bool> DeleteAsync(int id);
    }
}

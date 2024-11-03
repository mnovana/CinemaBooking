using FilmService.Models;
using SharedLibrary.Models.DTO;

namespace FilmService.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(int id);
        Task<MovieTitleDurationDTO?> GetTitleDurationByIdAsync(int id);
        Task AddAsync(Movie movie);
        Task<bool> UpdateAsync(Movie movie);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<MovieTitleDTO>> GetTitlesByIdsAsync(List<int> ids);
    }
}

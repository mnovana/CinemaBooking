using MovieService.Models;
using MovieService.Models.DTO;

namespace MovieService.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(int id);
        Task AddAsync(Movie movie);
        Task<bool> UpdateAsync(Movie movie);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<MovieTitleDTO>> GetTitlesByIdsAsync(List<int> ids);
    }
}

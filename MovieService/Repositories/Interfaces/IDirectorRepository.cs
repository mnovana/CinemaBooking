using MovieService.Models;

namespace MovieService.Repositories.Interfaces
{
    public interface IDirectorRepository
    {
        Task<IEnumerable<Director>> GetAllAsync();
        Task<Director?> GetByIdAsync(int id);
        Task AddAsync(Director director);
        Task<bool> UpdateAsync(Director director);
        Task<bool> DeleteAsync(int id);
    }
}

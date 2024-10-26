using ScreeningService.Models;

namespace ScreeningService.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat?> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetAllAsync();
        Task<IEnumerable<Seat>> GetByIdsAsync(int[] ids);
    }
}

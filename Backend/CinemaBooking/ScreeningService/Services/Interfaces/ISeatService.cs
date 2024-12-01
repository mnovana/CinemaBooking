using SharedLibrary.Models.DTO;

namespace ScreeningService.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDTO>> GetAllAsync();
        Task<SeatDTO?> GetByIdAsync(int id);
        Task<IEnumerable<SeatDTO>> GetByIdsAsync(int[] ids);
        Task<IEnumerable<SeatDTO>> GetByIdsIfRoomMatchesAsync(int[] ids, int screeningRoomNumber);

    }
}

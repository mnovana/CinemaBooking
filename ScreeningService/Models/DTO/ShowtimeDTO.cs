using SharedLibrary.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace ScreeningService.Models.DTO
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int ScreeningRoomNumber { get; set; }

        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
    }
}

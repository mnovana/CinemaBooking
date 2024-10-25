using System.ComponentModel.DataAnnotations;

namespace ScreeningService.Models
{
    public class Row
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }

        [Required]
        public int ScreeningRoomId { get; set; }
        public ScreeningRoom? ScreeningRoom { get; set; }
    }
}

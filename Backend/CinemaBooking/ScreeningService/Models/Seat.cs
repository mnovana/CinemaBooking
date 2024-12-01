using System.ComponentModel.DataAnnotations;

namespace ScreeningService.Models
{
    public class Seat
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }

        [Required]
        public int RowId { get; set; }
        public Row? Row { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ScreeningService.Models
{
    public class ScreeningRoom
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }
    }
}

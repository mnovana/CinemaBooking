using SharedLibrary.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace ScreeningService.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        [Required]
        [MinDate(2024, 1, 1)]
        [MaxYearsAfterNow(2)]
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }       // End is calculated using movie duration
                                                // and is needed when adding a new showtime
                                                // to check if showtimes overlap 
        [Required]
        public int ScreeningRoomId { get; set; }
        public ScreeningRoom? ScreeningRoom { get; set; }

        // from FilmService
        [Required]
        public int MovieId { get; set; }
    }
}

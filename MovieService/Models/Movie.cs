using MovieService.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace MovieService.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        [Range(1, 300)]
        public int Duration { get; set; }
        [Required]
        [DateInRange]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public int DirectorId { get; set; }
        public Director? Director { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

    }
}

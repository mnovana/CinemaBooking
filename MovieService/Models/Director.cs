using System.ComponentModel.DataAnnotations;

namespace MovieService.Models
{
    public class Director
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
    }
}

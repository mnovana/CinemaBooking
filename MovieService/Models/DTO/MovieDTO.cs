namespace MovieService.Models.DTO
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string DirectorName { get; set; }
        public string GenreName { get; set; }
    }
}

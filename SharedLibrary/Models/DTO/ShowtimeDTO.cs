namespace SharedLibrary.Models.DTO
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }

        public int ScreeningRoomNumber { get; set; }

        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
    }
}

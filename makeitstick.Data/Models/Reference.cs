namespace makeitstick.Models
{
    // Bronvermelding bij een hoofdstuk (boek, artikel, website).
    public class Reference
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Author { get; set; }
        public string? Url { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; } = null!;
    }
}

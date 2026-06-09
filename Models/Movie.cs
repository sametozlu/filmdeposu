namespace FilmSerileri.Models;

public class Movie
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int DurationMinutes { get; set; }
    public string Synopsis { get; set; } = string.Empty;
    public double ImdbRating { get; set; }
}

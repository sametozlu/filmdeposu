namespace FilmSerileri.Models;

public class MovieSeries
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYearStart { get; set; }
    public int ReleaseYearEnd { get; set; }
    public string Director { get; set; } = string.Empty;
    public string Studio { get; set; } = string.Empty;
    public double ImdbRating { get; set; }
    public string AccentColor { get; set; } = "#e50914";
    public string GradientFrom { get; set; } = "#1a1a2e";
    public string GradientTo { get; set; } = "#16213e";
    public string Icon { get; set; } = "🎬";
    public string PosterUrl { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public string GenreKey { get; set; } = string.Empty;
    public List<Movie> Movies { get; set; } = new();
    public List<CastMember> Cast { get; set; } = new();
}

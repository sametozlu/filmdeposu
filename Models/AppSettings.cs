namespace FilmSerileri.Models;

public class AppSettings
{
    public string Theme { get; set; } = "dark";
    public string Language { get; set; } = "tr";
    public bool ShowRatings { get; set; } = true;
    public bool CompactView { get; set; } = false;
    public string FavoriteSeriesId { get; set; } = string.Empty;
}

using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class HomeViewModel
{
    public List<MovieSeries> Series { get; set; } = new();
    public MovieSeries? FeaturedSeries { get; set; }
    public AppSettings Settings { get; set; } = new();
    public string? Query { get; set; }
    public string? Genre { get; set; }
    public double? MinRating { get; set; }
    public string SortBy { get; set; } = "rating";
    public List<(string Key, string Label)> Genres { get; set; } = new();
    public int TotalCount { get; set; }
    public List<MovieSeries> MarqueeSeries { get; set; } = new();
}

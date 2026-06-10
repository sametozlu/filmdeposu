using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class HomeViewModel
{
  public PagedResult<MovieSeries> PagedSeries { get; set; } = new();
  public List<MovieSeries> Series => PagedSeries.Items.ToList();
  public MovieSeries? FeaturedSeries { get; set; }
  public AppSettings Settings { get; set; } = new();
  public string? Query { get; set; }
  public string? Genre { get; set; }
  public double? MinRating { get; set; }
  public string SortBy { get; set; } = "rating";
  public int Page { get; set; } = 1;
  public List<(string Key, string Label)> Genres { get; set; } = new();
  public int TotalCount => PagedSeries.TotalCount;
  public List<MovieSeries> MarqueeSeries { get; set; } = new();
  public IReadOnlyList<MovieSeries> Recommended { get; set; } = Array.Empty<MovieSeries>();
}

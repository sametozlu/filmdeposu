using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class CompareViewModel
{
  public AppSettings Settings { get; set; } = new();
  public List<MovieSeries> AllSeries { get; set; } = new();
  public string? SeriesAId { get; set; }
  public string? SeriesBId { get; set; }
  public SeriesComparison? Comparison { get; set; }
}

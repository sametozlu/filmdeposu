using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class LibraryViewModel
{
  public AppSettings Settings { get; set; } = new();
  public List<MovieSeries> Watchlist { get; set; } = new();
  public List<(MovieSeries Series, int? MovieOrder)> Watched { get; set; } = new();
}

using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class DashboardController : Controller
{
  private readonly IMovieService _movies;
  private readonly ISettingsService _settings;

  public DashboardController(IMovieService movies, ISettingsService settings)
  {
    _movies = movies;
    _settings = settings;
  }

  public IActionResult Index()
  {
    var lang = _settings.GetSettings().Language;
    var all = _movies.GetAllSeries(lang);

    var genres = all
      .GroupBy(s => s.Genre)
      .OrderByDescending(g => g.Count())
      .ToList();

    var decades = all
      .SelectMany(s => s.Movies)
      .GroupBy(m => m.Year / 10 * 10)
      .OrderBy(g => g.Key)
      .ToList();

    var top = all.OrderByDescending(s => s.ImdbRating).Take(8).ToList();

    var model = new DashboardViewModel
    {
      SeriesCount = all.Count,
      MovieCount = all.Sum(s => s.Movies.Count),
      ActorCount = all.SelectMany(s => s.Cast.Select(c => c.ActorName)).Distinct().Count(),
      AverageRating = Math.Round(all.Average(s => s.ImdbRating), 2),
      GenreLabels = genres.Select(g => g.Key).ToList(),
      GenreCounts = genres.Select(g => g.Count()).ToList(),
      DecadeLabels = decades.Select(d => $"{d.Key}s").ToList(),
      DecadeCounts = decades.Select(d => d.Count()).ToList(),
      TopTitles = top.Select(s => s.Title).ToList(),
      TopRatings = top.Select(s => s.ImdbRating).ToList(),
      TopColors = top.Select(s => s.AccentColor).ToList()
    };

    return View(model);
  }
}

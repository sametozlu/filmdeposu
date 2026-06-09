using System.Diagnostics;
using FilmSerileri.Models;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class HomeController : Controller
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settingsService;

  public HomeController(IMovieService movieService, ISettingsService settingsService)
  {
    _movieService = movieService;
    _settingsService = settingsService;
  }

  public IActionResult Index(string? q, string? genre, double? minRating, string? sort)
  {
    var settings = _settingsService.GetSettings();
    var lang = settings.Language;
    var sortBy = string.IsNullOrWhiteSpace(sort) ? "rating" : sort;

    var series = _movieService
      .SearchSeries(q, genre, minRating, sortBy, lang)
      .ToList();

    if (!string.IsNullOrEmpty(settings.FavoriteSeriesId))
    {
      var favorite = series.FirstOrDefault(s => s.Id == settings.FavoriteSeriesId);
      if (favorite != null)
      {
        series.Remove(favorite);
        series.Insert(0, favorite);
      }
    }

    var model = new HomeViewModel
    {
      Series = series,
      FeaturedSeries = _movieService.GetFeaturedSeries(lang),
      Settings = settings,
      Query = q,
      Genre = genre,
      MinRating = minRating,
      SortBy = sortBy,
      Genres = _movieService.GetGenres(lang).ToList(),
      TotalCount = series.Count,
      MarqueeSeries = _movieService.GetAllSeries(lang).ToList()
    };

    return View(model);
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}

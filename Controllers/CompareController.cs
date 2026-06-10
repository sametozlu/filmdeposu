using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class CompareController : Controller
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settingsService;

  public CompareController(IMovieService movieService, ISettingsService settingsService)
  {
    _movieService = movieService;
    _settingsService = settingsService;
  }

  public IActionResult Index(string? a, string? b)
  {
    var settings = _settingsService.GetSettings();
    var lang = settings.Language;
    var model = new CompareViewModel
    {
      Settings = settings,
      AllSeries = _movieService.GetAllSeries(lang).ToList(),
      SeriesAId = a,
      SeriesBId = b
    };

    if (!string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(b))
      model.Comparison = _movieService.CompareSeries(a, b, lang);

    return View(model);
  }
}

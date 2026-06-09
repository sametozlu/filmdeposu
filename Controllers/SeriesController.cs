using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class SeriesController : Controller
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settingsService;

  public SeriesController(IMovieService movieService, ISettingsService settingsService)
  {
    _movieService = movieService;
    _settingsService = settingsService;
  }

  public IActionResult Detail(string id)
  {
    var settings = _settingsService.GetSettings();
    var lang = settings.Language;
    var series = _movieService.GetSeriesById(id, lang);
    if (series == null)
      return NotFound();

    var all = _movieService.GetAllSeries(lang);
    var index = all.ToList().FindIndex(s => s.Id == id);

    var model = new SeriesDetailViewModel
    {
      Series = series,
      Settings = settings,
      PrevSeries = index > 0 ? all[index - 1] : null,
      NextSeries = index < all.Count - 1 ? all[index + 1] : null
    };

    return View(model);
  }
}

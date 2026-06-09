using FilmSerileri.Models;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class SettingsController : Controller
{
  private readonly ISettingsService _settingsService;
  private readonly IMovieService _movieService;

  public SettingsController(ISettingsService settingsService, IMovieService movieService)
  {
    _settingsService = settingsService;
    _movieService = movieService;
  }

  public IActionResult Index()
  {
    var model = new SettingsViewModel
    {
      Settings = _settingsService.GetSettings(),
      AllSeries = _movieService.GetAllSeries().ToList()
    };

    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Index(AppSettings settings)
  {
    _settingsService.SaveSettings(settings);

    TempData["Success"] = "Ayarlarınız kaydedildi!";
    return RedirectToAction(nameof(Index));
  }
}

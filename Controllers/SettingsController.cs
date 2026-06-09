using FilmSerileri.Models;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class SettingsController : Controller
{
  private readonly ISettingsService _settingsService;
  private readonly IMovieService _movieService;
  private readonly ILocalizationService _localization;

  public SettingsController(ISettingsService settingsService, IMovieService movieService, ILocalizationService localization)
  {
    _settingsService = settingsService;
    _movieService = movieService;
    _localization = localization;
  }

  public IActionResult Index()
  {
    var settings = _settingsService.GetSettings();
    var model = new SettingsViewModel
    {
      Settings = settings,
      AllSeries = _movieService.GetAllSeries(settings.Language).ToList()
    };

    return View(model);
  }

  [HttpPost]
  public IActionResult SetLanguage(string lang, string? returnUrl)
  {
    var settings = _settingsService.GetSettings();
    settings.Language = lang is "en" or "tr" ? lang : "tr";
    _settingsService.SaveSettings(settings);
    return LocalRedirect(returnUrl ?? "/");
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Index(AppSettings settings)
  {
    _settingsService.SaveSettings(settings);
    TempData["Success"] = _localization.T("settings_saved", settings.Language);
    return RedirectToAction(nameof(Index));
  }
}

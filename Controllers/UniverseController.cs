using FilmSerileri.Data;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class UniverseController : Controller
{
  private readonly ISettingsService _settingsService;

  public UniverseController(ISettingsService settingsService) => _settingsService = settingsService;

  public IActionResult Index()
  {
    var settings = _settingsService.GetSettings();
    return View(new UniverseIndexViewModel
    {
      Settings = settings,
      Maps = UniverseCatalog.GetAll(settings.Language)
    });
  }

  public IActionResult Detail(string id)
  {
    var settings = _settingsService.GetSettings();
    var map = UniverseCatalog.GetById(id, settings.Language);
    if (map == null) return NotFound();
    return View(new UniverseDetailViewModel { Settings = settings, Map = map });
  }
}

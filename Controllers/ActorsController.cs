using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class ActorsController : Controller
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settingsService;

  public ActorsController(IMovieService movieService, ISettingsService settingsService)
  {
    _movieService = movieService;
    _settingsService = settingsService;
  }

  public IActionResult Index()
  {
    var settings = _settingsService.GetSettings();
    return View(new ActorsIndexViewModel
    {
      Settings = settings,
      Actors = _movieService.GetAllActors(settings.Language)
    });
  }

  public async Task<IActionResult> Detail(string id)
  {
    var settings = _settingsService.GetSettings();
    var actor = await _movieService.GetActorBySlugAsync(id, settings.Language);
    if (actor == null) return NotFound();

    return View(new ActorDetailViewModel { Settings = settings, Actor = actor });
  }
}

using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

[Authorize]
public class LibraryController : Controller
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settingsService;
  private readonly IUserLibraryService _library;

  public LibraryController(IMovieService movieService, ISettingsService settingsService, IUserLibraryService library)
  {
    _movieService = movieService;
    _settingsService = settingsService;
    _library = library;
  }

  public async Task<IActionResult> Index()
  {
    var settings = _settingsService.GetSettings();
    var lang = settings.Language;
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

    var watchlistIds = await _library.GetWatchlistAsync(userId);
    var watched = await _library.GetWatchedAsync(userId);

    var watchlist = new List<Models.MovieSeries>();
    foreach (var id in watchlistIds)
    {
      var s = _movieService.GetSeriesById(id, lang);
      if (s != null) watchlist.Add(s);
    }

    var watchedList = new List<(Models.MovieSeries Series, int? MovieOrder)>();
    foreach (var w in watched)
    {
      var s = _movieService.GetSeriesById(w.SeriesId, lang);
      if (s != null) watchedList.Add((s, w.MovieOrder));
    }

    return View(new LibraryViewModel
    {
      Settings = settings,
      Watchlist = watchlist,
      Watched = watchedList
    });
  }
}

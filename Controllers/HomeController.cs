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
  private readonly IRecommendationService _recommendations;

  public HomeController(IMovieService movieService, ISettingsService settingsService, IRecommendationService recommendations)
  {
    _movieService = movieService;
    _settingsService = settingsService;
    _recommendations = recommendations;
  }

  public async Task<IActionResult> Index(string? q, string? genre, double? minRating, string? sort, int page = 1)
  {
    var settings = _settingsService.GetSettings();
    var lang = settings.Language;
    var sortBy = string.IsNullOrWhiteSpace(sort) ? "rating" : sort;

    var paged = _movieService.SearchSeriesPaged(q, genre, minRating, sortBy, page, 9, lang);
    var series = paged.Items.ToList();

    if (!string.IsNullOrEmpty(settings.FavoriteSeriesId) && page == 1 && string.IsNullOrWhiteSpace(q) && string.IsNullOrWhiteSpace(genre))
    {
      var favorite = series.FirstOrDefault(s => s.Id == settings.FavoriteSeriesId)
        ?? _movieService.GetSeriesById(settings.FavoriteSeriesId, lang);
      if (favorite != null && !series.Any(s => s.Id == favorite.Id))
      {
        series.Insert(0, favorite);
        if (series.Count > paged.PageSize) series.RemoveAt(series.Count - 1);
      }
      else if (favorite != null)
      {
        series.RemoveAll(s => s.Id == favorite.Id);
        series.Insert(0, favorite);
      }
    }

    var model = new HomeViewModel
    {
      PagedSeries = new PagedResult<MovieSeries>
      {
        Items = series,
        Page = paged.Page,
        PageSize = paged.PageSize,
        TotalCount = paged.TotalCount
      },
      FeaturedSeries = _movieService.GetFeaturedSeries(lang),
      Settings = settings,
      Query = q,
      Genre = genre,
      MinRating = minRating,
      SortBy = sortBy,
      Page = page,
      Genres = _movieService.GetGenres(lang).ToList(),
      MarqueeSeries = _movieService.GetAllSeries(lang).ToList()
    };

    if (User.Identity?.IsAuthenticated == true)
    {
      var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (userId != null)
        model.Recommended = await _recommendations.GetForUserAsync(userId, lang);
    }

    return View(model);
  }

  public IActionResult Random()
  {
    var lang = _settingsService.GetSettings().Language;
    var series = _movieService.GetRandomSeries(lang);
    return RedirectToAction("Detail", "Series", new { id = series.Id });
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error() =>
    View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

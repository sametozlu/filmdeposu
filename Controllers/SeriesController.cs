using FilmSerileri.Hubs;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FilmSerileri.Controllers;

public class SeriesController : Controller
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settingsService;
  private readonly IUserLibraryService _library;
  private readonly IReviewService _reviews;
  private readonly IHubContext<NotificationHub> _hub;

  public SeriesController(IMovieService movieService, ISettingsService settingsService, IUserLibraryService library,
    IReviewService reviews, IHubContext<NotificationHub> hub)
  {
    _movieService = movieService;
    _settingsService = settingsService;
    _library = library;
    _reviews = reviews;
    _hub = hub;
  }

  public async Task<IActionResult> Detail(string id)
  {
    var settings = _settingsService.GetSettings();
    var lang = settings.Language;
    var series = await _movieService.GetSeriesByIdAsync(id, lang);
    if (series == null) return NotFound();

    var all = _movieService.GetAllSeries(lang);
    var list = all.ToList();
    var index = list.FindIndex(s => s.Id == id);

    var model = new SeriesDetailViewModel
    {
      Series = series,
      Settings = settings,
      PrevSeries = index > 0 ? list[index - 1] : null,
      NextSeries = index < list.Count - 1 ? list[index + 1] : null,
      SimilarSeries = _movieService.GetSimilarSeries(id, lang),
      IsAuthenticated = User.Identity?.IsAuthenticated == true
    };

    model.Reviews = await _reviews.GetForSeriesAsync(id);
    model.AverageUserRating = await _reviews.GetAverageRatingAsync(id);

    if (model.IsAuthenticated && User.Identity?.Name != null)
    {
      var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (userId != null)
      {
        model.IsInWatchlist = await _library.IsInWatchlistAsync(userId, id);
        model.UserNote = await _library.GetNoteAsync(userId, id);
        model.UserReview = await _reviews.GetUserReviewAsync(userId, id);
      }
    }

    return View(model);
  }

  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> SaveReview(string id, int rating, string? text)
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();

    var author = User.FindFirst("DisplayName")?.Value ?? User.Identity?.Name ?? "Anonim";
    await _reviews.SaveAsync(userId, author, id, rating, text ?? string.Empty);

    var seriesTitle = _movieService.GetSeriesById(id)?.Title ?? id;
    await _hub.Clients.All.SendAsync("reviewPosted", new
    {
      seriesId = id,
      seriesTitle,
      author,
      rating,
      userId
    });

    return RedirectToAction(nameof(Detail), "Series", new { id }, "reviews");
  }

  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteReview(string id)
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();
    await _reviews.DeleteAsync(userId, id);
    return RedirectToAction(nameof(Detail), new { id });
  }

  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> ToggleWatchlist(string id)
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();
    await _library.ToggleWatchlistAsync(userId, id);
    return RedirectToAction(nameof(Detail), new { id });
  }

  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> MarkWatched(string id, int? movieOrder)
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();
    await _library.MarkWatchedAsync(userId, id, movieOrder);
    return RedirectToAction(nameof(Detail), new { id });
  }

  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> SaveNote(string id, string note)
  {
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();
    await _library.SaveNoteAsync(userId, id, note);
    return RedirectToAction(nameof(Detail), new { id });
  }
}

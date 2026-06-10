using System.Security.Claims;
using FilmSerileri.Data;
using FilmSerileri.Entities;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Controllers;

[Authorize]
public class ProfileController : Controller
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IUserLibraryService _library;
  private readonly IMovieService _movies;
  private readonly ApplicationDbContext _db;
  private readonly ISettingsService _settings;

  public ProfileController(UserManager<ApplicationUser> userManager, IUserLibraryService library,
    IMovieService movies, ApplicationDbContext db, ISettingsService settings)
  {
    _userManager = userManager;
    _library = library;
    _movies = movies;
    _db = db;
    _settings = settings;
  }

  public async Task<IActionResult> Index()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return Unauthorized();

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return NotFound();

    var lang = _settings.GetSettings().Language;
    var watchlist = await _library.GetWatchlistAsync(userId);
    var watched = await _library.GetWatchedAsync(userId);
    var watchedSeries = watched.Select(w => w.SeriesId).Distinct().ToList();
    var myReviews = await _db.Reviews.AsNoTracking().Where(r => r.UserId == userId).ToListAsync();
    var bestQuiz = await _db.QuizScores.AsNoTracking()
      .Where(q => q.UserId == userId)
      .OrderByDescending(q => q.Score)
      .Select(q => (int?)q.Score)
      .FirstOrDefaultAsync() ?? 0;

    var genreCount = watchlist.Concat(watchedSeries).Distinct()
      .Select(id => _movies.GetSeriesById(id, lang)?.GenreKey)
      .Where(g => g != null)
      .Distinct()
      .Count();

    var model = new ProfileViewModel
    {
      DisplayName = user.DisplayName ?? user.UserName ?? "Anonim",
      Email = user.Email ?? string.Empty,
      MemberSince = user.CreatedAt,
      WatchlistCount = watchlist.Count,
      WatchedCount = watchedSeries.Count,
      ReviewCount = myReviews.Count,
      GenreCount = genreCount,
      BestQuizScore = bestQuiz,
      Badges = new List<BadgeViewModel>
      {
        new() { Icon = "🌱", TitleKey = "badge_first_step", DescriptionKey = "badge_first_step_desc", Earned = watchedSeries.Count >= 1 },
        new() { Icon = "🎬", TitleKey = "badge_marathon", DescriptionKey = "badge_marathon_desc", Earned = watchedSeries.Count >= 3 },
        new() { Icon = "🏆", TitleKey = "badge_cinephile", DescriptionKey = "badge_cinephile_desc", Earned = watchedSeries.Count >= 7 },
        new() { Icon = "📝", TitleKey = "badge_first_review", DescriptionKey = "badge_first_review_desc", Earned = myReviews.Count >= 1 },
        new() { Icon = "✍️", TitleKey = "badge_critic", DescriptionKey = "badge_critic_desc", Earned = myReviews.Count >= 5 },
        new() { Icon = "📚", TitleKey = "badge_collector", DescriptionKey = "badge_collector_desc", Earned = watchlist.Count >= 5 },
        new() { Icon = "🦉", TitleKey = "badge_explorer", DescriptionKey = "badge_explorer_desc", Earned = genreCount >= 4 },
        new() { Icon = "🧠", TitleKey = "badge_quiz_master", DescriptionKey = "badge_quiz_master_desc", Earned = bestQuiz >= 8 },
      }
    };

    return View(model);
  }
}

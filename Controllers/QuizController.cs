using System.Security.Claims;
using FilmSerileri.Data;
using FilmSerileri.Entities;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Controllers;

public class QuizController : Controller
{
  private readonly IMovieService _movies;
  private readonly ISettingsService _settings;
  private readonly ApplicationDbContext _db;
  private readonly ILocalizationService _l;

  public QuizController(IMovieService movies, ISettingsService settings, ApplicationDbContext db, ILocalizationService l)
  {
    _movies = movies;
    _settings = settings;
    _db = db;
    _l = l;
  }

  public async Task<IActionResult> Index()
  {
    var lang = _settings.GetSettings().Language;
    var model = new QuizViewModel
    {
      Questions = GenerateQuestions(lang, 10),
      Leaderboard = await _db.QuizScores.AsNoTracking()
        .OrderByDescending(q => q.Score).ThenBy(q => q.CreatedAt)
        .Take(10)
        .ToListAsync(),
      IsAuthenticated = User.Identity?.IsAuthenticated == true
    };
    return View(model);
  }

  [Authorize, HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> SaveScore(int score, int total)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null || score < 0 || total <= 0 || score > total) return BadRequest();

    var author = User.FindFirst("DisplayName")?.Value ?? User.Identity?.Name ?? "Anonim";
    var best = await _db.QuizScores.FirstOrDefaultAsync(q => q.UserId == userId);

    if (best == null)
      _db.QuizScores.Add(new QuizScore { UserId = userId, AuthorName = author, Score = score, Total = total });
    else if (score > best.Score)
    {
      best.Score = score;
      best.Total = total;
      best.CreatedAt = DateTime.UtcNow;
    }

    await _db.SaveChangesAsync();
    return Ok();
  }

  private List<QuizQuestion> GenerateQuestions(string lang, int count)
  {
    var all = _movies.GetAllSeries(lang);
    var rng = Random.Shared;
    var questions = new List<QuizQuestion>();

    var allActors = all.SelectMany(s => s.Cast.Select(c => c.ActorName)).Distinct().ToList();
    var allTitles = all.Select(s => s.Title).ToList();
    var allYears = all.SelectMany(s => s.Movies.Select(m => m.Year)).Distinct().ToList();
    var allDirectors = all.Select(s => s.Director).Distinct().ToList();

    List<string> Distract(List<string> pool, string correct, int n) =>
      pool.Where(x => x != correct).OrderBy(_ => rng.Next()).Take(n).ToList();

    while (questions.Count < count)
    {
      var s = all[rng.Next(all.Count)];
      switch (rng.Next(4))
      {
        case 0 when s.Cast.Count > 0:
        {
          var c = s.Cast[rng.Next(s.Cast.Count)];
          var options = Distract(allActors, c.ActorName, 3).Append(c.ActorName).OrderBy(_ => rng.Next()).ToList();
          if (options.Count < 4) continue;
          questions.Add(new QuizQuestion(
            string.Format(_l.T("quiz_q_actor", lang), c.CharacterName, s.Title),
            options, options.IndexOf(c.ActorName)));
          break;
        }
        case 1 when s.Movies.Count > 0:
        {
          var m = s.Movies[rng.Next(s.Movies.Count)];
          var years = Distract(allYears.Select(y => y.ToString()).ToList(), m.Year.ToString(), 3)
            .Append(m.Year.ToString()).OrderBy(_ => rng.Next()).ToList();
          if (years.Count < 4) continue;
          questions.Add(new QuizQuestion(
            string.Format(_l.T("quiz_q_year", lang), m.Title),
            years, years.IndexOf(m.Year.ToString())));
          break;
        }
        case 2 when s.Movies.Count > 0:
        {
          var m = s.Movies[rng.Next(s.Movies.Count)];
          var options = Distract(allTitles, s.Title, 3).Append(s.Title).OrderBy(_ => rng.Next()).ToList();
          if (options.Count < 4) continue;
          questions.Add(new QuizQuestion(
            string.Format(_l.T("quiz_q_series", lang), m.Title),
            options, options.IndexOf(s.Title)));
          break;
        }
        case 3:
        {
          var options = Distract(allDirectors, s.Director, 3).Append(s.Director).OrderBy(_ => rng.Next()).ToList();
          if (options.Count < 4) continue;
          questions.Add(new QuizQuestion(
            string.Format(_l.T("quiz_q_director", lang), s.Title),
            options, options.IndexOf(s.Director)));
          break;
        }
      }
      // Aynı sorunun tekrarını engelle
      questions = questions.DistinctBy(q => q.Text).ToList();
    }

    return questions;
  }
}

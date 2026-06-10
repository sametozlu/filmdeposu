using FilmSerileri.Data;
using FilmSerileri.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FilmSerileri.Controllers.Api;

[ApiController]
[Route("api/v1/search")]
[EnableRateLimiting("api")]
public class SearchApiController : ControllerBase
{
  private readonly IMovieService _movies;
  private readonly ISettingsService _settings;

  public SearchApiController(IMovieService movies, ISettingsService settings)
  {
    _movies = movies;
    _settings = settings;
  }

  /// <summary>Komut paleti için seri, film ve oyuncu karışık hızlı arama.</summary>
  [HttpGet]
  public IActionResult Search([FromQuery] string? q)
  {
    if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
      return Ok(Array.Empty<object>());

    var lang = _settings.GetSettings().Language;
    var term = q.Trim();
    var all = _movies.GetAllSeries(lang);
    var results = new List<object>();

    bool Hit(string? text) => text?.Contains(term, StringComparison.OrdinalIgnoreCase) == true;

    foreach (var s in all)
    {
      if (Hit(s.Title) || Hit(s.OriginalTitle))
        results.Add(new { type = "series", title = s.Title, subtitle = s.Genre, url = $"/Series/Detail/{s.Id}", image = s.PosterUrl });
    }

    foreach (var s in all)
    {
      foreach (var m in s.Movies.Where(m => Hit(m.Title)))
        results.Add(new { type = "movie", title = m.Title, subtitle = $"{s.Title} · {m.Year}", url = $"/Series/Detail/{s.Id}", image = m.PosterUrl });
    }

    var seenActors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var s in all)
    {
      foreach (var c in s.Cast.Where(c => Hit(c.ActorName) || Hit(c.CharacterName)))
      {
        if (!seenActors.Add(c.ActorName)) continue;
        results.Add(new { type = "actor", title = c.ActorName, subtitle = c.CharacterName, url = $"/Actors/Detail/{ActorSlug.FromName(c.ActorName)}", image = c.PhotoUrl });
      }
    }

    return Ok(results.Take(10));
  }
}

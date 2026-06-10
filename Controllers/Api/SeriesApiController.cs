using FilmSerileri.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FilmSerileri.Controllers.Api;

[ApiController]
[Route("api/v1/series")]
[EnableRateLimiting("api")]
public class SeriesApiController : ControllerBase
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settings;

  public SeriesApiController(IMovieService movieService, ISettingsService settings)
  {
    _movieService = movieService;
    _settings = settings;
  }

  [HttpGet]
  public IActionResult GetAll([FromQuery] string? q, [FromQuery] string? genre, [FromQuery] double? minRating,
    [FromQuery] string sort = "rating", [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
  {
    var lang = _settings.GetSettings().Language;
    if (!string.IsNullOrWhiteSpace(q) || !string.IsNullOrWhiteSpace(genre) || minRating.HasValue)
      return Ok(_movieService.SearchSeriesPaged(q, genre, minRating, sort, page, pageSize, lang));
    return Ok(new { items = _movieService.GetAllSeries(lang), totalCount = _movieService.GetAllSeries(lang).Count });
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
  {
    var lang = _settings.GetSettings().Language;
    var series = await _movieService.GetSeriesByIdAsync(id, lang);
    return series == null ? NotFound() : Ok(series);
  }

  [HttpGet("{id}/similar")]
  public IActionResult GetSimilar(string id)
  {
    var lang = _settings.GetSettings().Language;
    return Ok(_movieService.GetSimilarSeries(id, lang));
  }

  [HttpGet("random")]
  public IActionResult GetRandom()
  {
    var lang = _settings.GetSettings().Language;
    return Ok(_movieService.GetRandomSeries(lang));
  }
}

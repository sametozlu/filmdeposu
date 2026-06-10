using FilmSerileri.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FilmSerileri.Controllers.Api;

[ApiController]
[Route("api/v1/actors")]
[EnableRateLimiting("api")]
public class ActorsApiController : ControllerBase
{
  private readonly IMovieService _movieService;
  private readonly ISettingsService _settings;

  public ActorsApiController(IMovieService movieService, ISettingsService settings)
  {
    _movieService = movieService;
    _settings = settings;
  }

  [HttpGet]
  public IActionResult GetAll() =>
    Ok(_movieService.GetAllActors(_settings.GetSettings().Language));

  [HttpGet("{slug}")]
  public async Task<IActionResult> GetBySlug(string slug)
  {
    var actor = await _movieService.GetActorBySlugAsync(slug, _settings.GetSettings().Language);
    return actor == null ? NotFound() : Ok(actor);
  }
}

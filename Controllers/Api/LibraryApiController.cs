using System.Security.Claims;
using FilmSerileri.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FilmSerileri.Controllers.Api;

[ApiController]
[Route("api/v1/library")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[EnableRateLimiting("api")]
public class LibraryApiController : ControllerBase
{
  private readonly IUserLibraryService _library;

  public LibraryApiController(IUserLibraryService library) => _library = library;

  private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

  [HttpGet("watchlist")]
  public async Task<IActionResult> GetWatchlist()
  {
    if (UserId == null) return Unauthorized();
    return Ok(await _library.GetWatchlistAsync(UserId));
  }

  [HttpPost("watchlist/{seriesId}")]
  public async Task<IActionResult> ToggleWatchlist(string seriesId)
  {
    if (UserId == null) return Unauthorized();
    var added = await _library.ToggleWatchlistAsync(UserId, seriesId);
    return Ok(new { seriesId, inWatchlist = added });
  }

  [HttpGet("watched")]
  public async Task<IActionResult> GetWatched()
  {
    if (UserId == null) return Unauthorized();
    var watched = await _library.GetWatchedAsync(UserId);
    return Ok(watched.Select(w => new { w.SeriesId, w.MovieOrder }));
  }
}

using FilmSerileri.Data;
using FilmSerileri.Models;
using FilmSerileri.Options;
using FilmSerileri.Services.Omdb;
using FilmSerileri.Services.Tmdb;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FilmSerileri.Services;

public class MovieEnrichmentService
{
  private readonly ITmdbService _tmdb;
  private readonly IOmdbService _omdb;
  private readonly TmdbOptions _tmdbOptions;
  private readonly IMemoryCache _cache;
  private readonly ILogger<MovieEnrichmentService> _logger;

  public MovieEnrichmentService(
    ITmdbService tmdb,
    IOmdbService omdb,
    IOptions<TmdbOptions> tmdbOptions,
    IMemoryCache cache,
    ILogger<MovieEnrichmentService> logger)
  {
    _tmdb = tmdb;
    _omdb = omdb;
    _tmdbOptions = tmdbOptions.Value;
    _cache = cache;
    _logger = logger;
  }

  public MovieSeries Enrich(MovieSeries series)
  {
    series.TmdbCollectionId ??= TmdbCatalog.GetCollectionId(series.Id);
    series.UniverseId ??= series.Id switch
    {
      "yildiz-savaslari" => "star-wars",
      "orumcek-adam" => "mcu-spider",
      "yuzuklerin-efendisi" => "middle-earth",
      _ => null
    };

    foreach (var movie in series.Movies)
    {
      movie.TmdbId ??= TmdbCatalog.GetMovieId(series.Id, movie.Order);
    }

    return series;
  }

  public async Task<MovieSeries> EnrichAsync(MovieSeries series, CancellationToken ct = default)
  {
    var copy = CloneSeries(series);
    Enrich(copy);

    foreach (var movie in copy.Movies)
    {
      if (movie.TmdbId is int tmdbId)
      {
        movie.TrailerYouTubeKey ??= await _cache.GetOrCreateAsync($"trailer:{tmdbId}", async entry =>
        {
          entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3);
          return await _tmdb.GetTrailerKeyAsync(tmdbId, ct);
        });

        if (_tmdb.IsAvailable)
        {
          var details = await _tmdb.GetMovieAsync(tmdbId, ct);
          if (details != null)
          {
            if (details.VoteAverage > 0) movie.ImdbRating = Math.Round(details.VoteAverage, 1);
            if (!string.IsNullOrEmpty(details.PosterPath))
              movie.PosterUrl = $"{_tmdbOptions.ImageBaseUrl}{details.PosterPath}";
          }
        }
      }

      if (_omdb.IsAvailable)
      {
        var omdbRating = await _omdb.GetImdbRatingAsync(movie.Title, movie.Year, ct);
        if (omdbRating.HasValue) movie.ImdbRating = omdbRating.Value;
      }
    }

    if (copy.Movies.Count > 0)
      copy.ImdbRating = Math.Round(copy.Movies.Average(m => m.ImdbRating), 1);

    return copy;
  }

  public async Task<ActorProfile?> EnrichActorAsync(ActorProfile profile, CancellationToken ct = default)
  {
    if (!_tmdb.IsAvailable || profile.TmdbId is not int personId) return profile;

    var details = await _tmdb.GetPersonAsync(personId, ct);
    if (details == null) return profile;

    profile.Biography = details.Biography;
    profile.BirthPlace = details.PlaceOfBirth;
    if (!string.IsNullOrEmpty(details.ProfilePath))
      profile.PhotoUrl = $"{_tmdbOptions.ImageBaseUrl}{details.ProfilePath}";

    return profile;
  }

  private static MovieSeries CloneSeries(MovieSeries s) => new()
  {
    Id = s.Id,
    Title = s.Title,
    OriginalTitle = s.OriginalTitle,
    Tagline = s.Tagline,
    Description = s.Description,
    Genre = s.Genre,
    GenreKey = s.GenreKey,
    ReleaseYearStart = s.ReleaseYearStart,
    ReleaseYearEnd = s.ReleaseYearEnd,
    Director = s.Director,
    Studio = s.Studio,
    ImdbRating = s.ImdbRating,
    AccentColor = s.AccentColor,
    GradientFrom = s.GradientFrom,
    GradientTo = s.GradientTo,
    Icon = s.Icon,
    PosterUrl = s.PosterUrl,
    BackdropUrl = s.BackdropUrl,
    UniverseId = s.UniverseId,
    TmdbCollectionId = s.TmdbCollectionId,
    Movies = s.Movies.Select(m => new Movie
    {
      Order = m.Order,
      Title = m.Title,
      Year = m.Year,
      DurationMinutes = m.DurationMinutes,
      Synopsis = m.Synopsis,
      ImdbRating = m.ImdbRating,
      PosterUrl = m.PosterUrl,
      TmdbId = m.TmdbId,
      TrailerYouTubeKey = m.TrailerYouTubeKey
    }).ToList(),
    Cast = s.Cast.Select(c => new CastMember
    {
      ActorName = c.ActorName,
      CharacterName = c.CharacterName,
      Role = c.Role,
      PhotoUrl = c.PhotoUrl
    }).ToList()
  };
}

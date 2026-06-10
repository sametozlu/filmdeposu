using FilmSerileri.Models;

namespace FilmSerileri.Services;

public interface IMovieService
{
  IReadOnlyList<MovieSeries> GetAllSeries(string language = "tr");
  MovieSeries? GetSeriesById(string id, string language = "tr");
  Task<MovieSeries?> GetSeriesByIdAsync(string id, string language = "tr", CancellationToken ct = default);
  MovieSeries? GetFeaturedSeries(string language = "tr");
  IReadOnlyList<(string Key, string Label)> GetGenres(string language = "tr");
  IReadOnlyList<MovieSeries> SearchSeries(string? query, string? genre, double? minRating, string sortBy, string language = "tr");
  PagedResult<MovieSeries> SearchSeriesPaged(string? query, string? genre, double? minRating, string sortBy, int page, int pageSize, string language = "tr");
  IReadOnlyList<MovieSeries> GetSimilarSeries(string id, string language = "tr");
  MovieSeries GetRandomSeries(string language = "tr");
  ActorProfile? GetActorBySlug(string slug, string language = "tr");
  Task<ActorProfile?> GetActorBySlugAsync(string slug, string language = "tr", CancellationToken ct = default);
  IReadOnlyList<ActorSummary> GetAllActors(string language = "tr");
  SeriesComparison? CompareSeries(string idA, string idB, string language = "tr");
  Task WarmupCacheAsync(CancellationToken ct = default);
  void InvalidateEnrichedCache();
}

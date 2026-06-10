namespace FilmSerileri.Services.Tmdb;

public interface ITmdbService
{
  bool IsAvailable { get; }
  Task<TmdbMovieDetails?> GetMovieAsync(int movieId, CancellationToken ct = default);
  Task<string?> GetTrailerKeyAsync(int movieId, CancellationToken ct = default);
  Task<TmdbPersonDetails?> GetPersonAsync(int personId, CancellationToken ct = default);
  Task<IReadOnlyList<int>> GetSimilarMovieIdsAsync(int movieId, CancellationToken ct = default);
  Task<IReadOnlyList<int>> GetCollectionMovieIdsAsync(int collectionId, CancellationToken ct = default);
}

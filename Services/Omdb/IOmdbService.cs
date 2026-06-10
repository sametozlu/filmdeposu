namespace FilmSerileri.Services.Omdb;

public interface IOmdbService
{
  bool IsAvailable { get; }
  Task<double?> GetImdbRatingAsync(string title, int year, CancellationToken ct = default);
}

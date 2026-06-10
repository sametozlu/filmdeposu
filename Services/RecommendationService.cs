using FilmSerileri.Models;

namespace FilmSerileri.Services;

/// <summary>Izleme listesi ve izlenenlere gore tur agirlikli oneri skoru hesaplar.</summary>
public class RecommendationService : IRecommendationService
{
  private readonly IUserLibraryService _library;
  private readonly IMovieService _movies;

  public RecommendationService(IUserLibraryService library, IMovieService movies)
  {
    _library = library;
    _movies = movies;
  }

  public async Task<IReadOnlyList<MovieSeries>> GetForUserAsync(string userId, string language = "tr")
  {
    var watchlist = await _library.GetWatchlistAsync(userId);
    var watched = await _library.GetWatchedAsync(userId);
    var seen = watchlist.Concat(watched.Select(w => w.SeriesId)).ToHashSet(StringComparer.OrdinalIgnoreCase);

    var all = _movies.GetAllSeries(language);

    if (seen.Count == 0)
      return all.OrderByDescending(s => s.ImdbRating).Take(4).ToList();

    var genreWeights = seen
      .Select(id => all.FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase))?.GenreKey)
      .Where(g => g != null)
      .GroupBy(g => g!)
      .ToDictionary(g => g.Key, g => g.Count());

    return all
      .Where(s => !seen.Contains(s.Id))
      .Select(s => new
      {
        Series = s,
        Score = genreWeights.GetValueOrDefault(s.GenreKey) * 2.0 + s.ImdbRating / 2.0
      })
      .OrderByDescending(x => x.Score)
      .ThenByDescending(x => x.Series.ImdbRating)
      .Take(4)
      .Select(x => x.Series)
      .ToList();
  }
}

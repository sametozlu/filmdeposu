using FilmSerileri.Models;

namespace FilmSerileri.Services;

public interface IRecommendationService
{
  Task<IReadOnlyList<MovieSeries>> GetForUserAsync(string userId, string language = "tr");
}

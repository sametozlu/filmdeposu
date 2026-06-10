using FilmSerileri.Entities;

namespace FilmSerileri.Services;

public interface IReviewService
{
  Task<IReadOnlyList<Review>> GetForSeriesAsync(string seriesId);
  Task<Review?> GetUserReviewAsync(string userId, string seriesId);
  Task<double?> GetAverageRatingAsync(string seriesId);
  Task SaveAsync(string userId, string authorName, string seriesId, int rating, string text);
  Task DeleteAsync(string userId, string seriesId);
}

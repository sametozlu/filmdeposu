using FilmSerileri.Data;
using FilmSerileri.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Services;

public class ReviewService : IReviewService
{
  private readonly ApplicationDbContext _db;

  public ReviewService(ApplicationDbContext db) => _db = db;

  public async Task<IReadOnlyList<Review>> GetForSeriesAsync(string seriesId) =>
    await _db.Reviews.AsNoTracking()
      .Where(r => r.SeriesId == seriesId)
      .OrderByDescending(r => r.CreatedAt)
      .Take(50)
      .ToListAsync();

  public async Task<Review?> GetUserReviewAsync(string userId, string seriesId) =>
    await _db.Reviews.AsNoTracking()
      .FirstOrDefaultAsync(r => r.UserId == userId && r.SeriesId == seriesId);

  public async Task<double?> GetAverageRatingAsync(string seriesId)
  {
    var ratings = await _db.Reviews.Where(r => r.SeriesId == seriesId).Select(r => r.Rating).ToListAsync();
    return ratings.Count == 0 ? null : Math.Round(ratings.Average(), 1);
  }

  public async Task SaveAsync(string userId, string authorName, string seriesId, int rating, string text)
  {
    rating = Math.Clamp(rating, 1, 5);
    text = (text ?? string.Empty).Trim();
    if (text.Length > 2000) text = text[..2000];

    var existing = await _db.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.SeriesId == seriesId);
    if (existing == null)
    {
      _db.Reviews.Add(new Review
      {
        UserId = userId,
        AuthorName = authorName,
        SeriesId = seriesId,
        Rating = rating,
        Text = text
      });
    }
    else
    {
      existing.Rating = rating;
      existing.Text = text;
      existing.CreatedAt = DateTime.UtcNow;
    }

    await _db.SaveChangesAsync();
  }

  public async Task DeleteAsync(string userId, string seriesId)
  {
    var review = await _db.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.SeriesId == seriesId);
    if (review != null)
    {
      _db.Reviews.Remove(review);
      await _db.SaveChangesAsync();
    }
  }
}

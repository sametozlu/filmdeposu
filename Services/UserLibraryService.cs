using FilmSerileri.Data;
using FilmSerileri.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Services;

public class UserLibraryService : IUserLibraryService
{
  private readonly ApplicationDbContext _db;

  public UserLibraryService(ApplicationDbContext db) => _db = db;

  public async Task<IReadOnlyList<string>> GetWatchlistAsync(string userId) =>
    await _db.WatchlistItems.Where(w => w.UserId == userId).OrderByDescending(w => w.AddedAt)
      .Select(w => w.SeriesId).ToListAsync();

  public async Task<bool> ToggleWatchlistAsync(string userId, string seriesId)
  {
    var existing = await _db.WatchlistItems.FirstOrDefaultAsync(w => w.UserId == userId && w.SeriesId == seriesId);
    if (existing != null)
    {
      _db.WatchlistItems.Remove(existing);
      await _db.SaveChangesAsync();
      return false;
    }

    _db.WatchlistItems.Add(new WatchlistItem { UserId = userId, SeriesId = seriesId });
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> IsInWatchlistAsync(string userId, string seriesId) =>
    await _db.WatchlistItems.AnyAsync(w => w.UserId == userId && w.SeriesId == seriesId);

  public async Task MarkWatchedAsync(string userId, string seriesId, int? movieOrder = null)
  {
    _db.WatchedItems.Add(new WatchedItem
    {
      UserId = userId,
      SeriesId = seriesId,
      MovieOrder = movieOrder,
      WatchedAt = DateTime.UtcNow
    });
    await _db.SaveChangesAsync();
  }

  public async Task<IReadOnlyList<(string SeriesId, int? MovieOrder)>> GetWatchedAsync(string userId) =>
    await _db.WatchedItems.Where(w => w.UserId == userId).OrderByDescending(w => w.WatchedAt)
      .Select(w => new ValueTuple<string, int?>(w.SeriesId, w.MovieOrder)).ToListAsync();

  public async Task<string?> GetNoteAsync(string userId, string seriesId)
  {
    var note = await _db.UserNotes.FirstOrDefaultAsync(n => n.UserId == userId && n.SeriesId == seriesId);
    return note?.Note;
  }

  public async Task SaveNoteAsync(string userId, string seriesId, string note)
  {
    var existing = await _db.UserNotes.FirstOrDefaultAsync(n => n.UserId == userId && n.SeriesId == seriesId);
    if (existing == null)
    {
      _db.UserNotes.Add(new UserNote { UserId = userId, SeriesId = seriesId, Note = note });
    }
    else
    {
      existing.Note = note;
      existing.UpdatedAt = DateTime.UtcNow;
    }
    await _db.SaveChangesAsync();
  }
}

namespace FilmSerileri.Services;

public interface IUserLibraryService
{
  Task<IReadOnlyList<string>> GetWatchlistAsync(string userId);
  Task<bool> ToggleWatchlistAsync(string userId, string seriesId);
  Task<bool> IsInWatchlistAsync(string userId, string seriesId);
  Task MarkWatchedAsync(string userId, string seriesId, int? movieOrder = null);
  Task<IReadOnlyList<(string SeriesId, int? MovieOrder)>> GetWatchedAsync(string userId);
  Task<string?> GetNoteAsync(string userId, string seriesId);
  Task SaveNoteAsync(string userId, string seriesId, string note);
}

namespace FilmSerileri.Entities;

public class WatchlistItem
{
  public int Id { get; set; }
  public string UserId { get; set; } = string.Empty;
  public string SeriesId { get; set; } = string.Empty;
  public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

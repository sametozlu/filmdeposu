namespace FilmSerileri.Entities;

public class WatchedItem
{
  public int Id { get; set; }
  public string UserId { get; set; } = string.Empty;
  public string SeriesId { get; set; } = string.Empty;
  public int? MovieOrder { get; set; }
  public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}

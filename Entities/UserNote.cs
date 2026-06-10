namespace FilmSerileri.Entities;

public class UserNote
{
  public int Id { get; set; }
  public string UserId { get; set; } = string.Empty;
  public string SeriesId { get; set; } = string.Empty;
  public string Note { get; set; } = string.Empty;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

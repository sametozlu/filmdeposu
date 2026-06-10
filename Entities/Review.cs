namespace FilmSerileri.Entities;

public class Review
{
  public int Id { get; set; }
  public string UserId { get; set; } = string.Empty;
  public string AuthorName { get; set; } = string.Empty;
  public string SeriesId { get; set; } = string.Empty;
  public int Rating { get; set; }
  public string Text { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

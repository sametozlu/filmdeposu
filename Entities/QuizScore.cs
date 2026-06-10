namespace FilmSerileri.Entities;

public class QuizScore
{
  public int Id { get; set; }
  public string UserId { get; set; } = string.Empty;
  public string AuthorName { get; set; } = string.Empty;
  public int Score { get; set; }
  public int Total { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

using FilmSerileri.Entities;

namespace FilmSerileri.ViewModels;

public record QuizQuestion(string Text, List<string> Options, int CorrectIndex);

public class QuizViewModel
{
  public List<QuizQuestion> Questions { get; set; } = new();
  public List<QuizScore> Leaderboard { get; set; } = new();
  public bool IsAuthenticated { get; set; }
}

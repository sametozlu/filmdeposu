namespace FilmSerileri.ViewModels;

public class DashboardViewModel
{
  public int SeriesCount { get; set; }
  public int MovieCount { get; set; }
  public int ActorCount { get; set; }
  public double AverageRating { get; set; }
  public List<string> GenreLabels { get; set; } = new();
  public List<int> GenreCounts { get; set; } = new();
  public List<string> DecadeLabels { get; set; } = new();
  public List<int> DecadeCounts { get; set; } = new();
  public List<string> TopTitles { get; set; } = new();
  public List<double> TopRatings { get; set; } = new();
  public List<string> TopColors { get; set; } = new();
}

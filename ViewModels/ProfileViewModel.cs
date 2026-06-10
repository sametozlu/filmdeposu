namespace FilmSerileri.ViewModels;

public class ProfileViewModel
{
  public string DisplayName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public DateTime MemberSince { get; set; }
  public int WatchlistCount { get; set; }
  public int WatchedCount { get; set; }
  public int ReviewCount { get; set; }
  public int GenreCount { get; set; }
  public int BestQuizScore { get; set; }
  public List<BadgeViewModel> Badges { get; set; } = new();
}

public class BadgeViewModel
{
  public string Icon { get; set; } = string.Empty;
  public string TitleKey { get; set; } = string.Empty;
  public string DescriptionKey { get; set; } = string.Empty;
  public bool Earned { get; set; }
}

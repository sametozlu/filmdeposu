using System.ComponentModel.DataAnnotations;

namespace FilmSerileri.ViewModels;

public class AdminSeriesListItem
{
  public string Id { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public double ImdbRating { get; set; }
  public int MovieCount { get; set; }
  public int SortOrder { get; set; }
}

public class AdminSeriesForm
{
  public bool IsNew { get; set; }

  [Required, RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Sadece küçük harf, rakam ve tire kullanın.")]
  public string Id { get; set; } = string.Empty;

  [Required] public string Title { get; set; } = string.Empty;
  public string OriginalTitle { get; set; } = string.Empty;
  public string Tagline { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Genre { get; set; } = string.Empty;
  public string GenreKey { get; set; } = string.Empty;
  public int ReleaseYearStart { get; set; }
  public int ReleaseYearEnd { get; set; }
  public string Director { get; set; } = string.Empty;
  public string Studio { get; set; } = string.Empty;
  [Range(0, 10)] public double ImdbRating { get; set; }
  public string AccentColor { get; set; } = "#e50914";
  public string GradientFrom { get; set; } = "#1a1a2e";
  public string GradientTo { get; set; } = "#16213e";
  public string Icon { get; set; } = "🎬";
  public string? UniverseId { get; set; }
  public int SortOrder { get; set; }

  public List<AdminMovieRow> Movies { get; set; } = new();
  public List<AdminCastRow> Cast { get; set; } = new();
}

public class AdminMovieRow
{
  public int Order { get; set; }
  public string Title { get; set; } = string.Empty;
  public int Year { get; set; }
  public int DurationMinutes { get; set; }
  public string Synopsis { get; set; } = string.Empty;
  public double ImdbRating { get; set; }
}

public class AdminCastRow
{
  public string ActorName { get; set; } = string.Empty;
  public string CharacterName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
}

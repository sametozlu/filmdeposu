namespace FilmSerileri.Entities;

public class SeriesEntity
{
  public string Id { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string OriginalTitle { get; set; } = string.Empty;
  public string Tagline { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Genre { get; set; } = string.Empty;
  public string GenreKey { get; set; } = string.Empty;
  public int ReleaseYearStart { get; set; }
  public int ReleaseYearEnd { get; set; }
  public string Director { get; set; } = string.Empty;
  public string Studio { get; set; } = string.Empty;
  public double ImdbRating { get; set; }
  public string AccentColor { get; set; } = "#e50914";
  public string GradientFrom { get; set; } = "#1a1a2e";
  public string GradientTo { get; set; } = "#16213e";
  public string Icon { get; set; } = "🎬";
  public string? UniverseId { get; set; }
  public int SortOrder { get; set; }
  public List<SeriesMovieEntity> Movies { get; set; } = new();
  public List<SeriesCastEntity> Cast { get; set; } = new();
}

public class SeriesMovieEntity
{
  public int Order { get; set; }
  public string Title { get; set; } = string.Empty;
  public int Year { get; set; }
  public int DurationMinutes { get; set; }
  public string Synopsis { get; set; } = string.Empty;
  public double ImdbRating { get; set; }
}

public class SeriesCastEntity
{
  public string ActorName { get; set; } = string.Empty;
  public string CharacterName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
}

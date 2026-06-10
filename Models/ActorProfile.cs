namespace FilmSerileri.Models;

public class ActorProfile
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string PhotoUrl { get; set; } = string.Empty;
  public string? Biography { get; set; }
  public string? BirthPlace { get; set; }
  public int? TmdbId { get; set; }
  public List<ActorRole> Roles { get; set; } = new();
}

public class ActorRole
{
  public string SeriesId { get; set; } = string.Empty;
  public string SeriesTitle { get; set; } = string.Empty;
  public string CharacterName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string PosterUrl { get; set; } = string.Empty;
}

public class ActorSummary
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string PhotoUrl { get; set; } = string.Empty;
}

public class SeriesComparison
{
  public MovieSeries SeriesA { get; set; } = new();
  public MovieSeries SeriesB { get; set; } = new();
}

public class UniverseMap
{
  public string Id { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public List<UniverseNode> Nodes { get; set; } = new();
}

public class UniverseNode
{
  public string Id { get; set; } = string.Empty;
  public string Label { get; set; } = string.Empty;
  public string SeriesId { get; set; } = string.Empty;
  public int X { get; set; }
  public int Y { get; set; }
  public string Color { get; set; } = "#e50914";
}

public class PagedResult<T>
{
  public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
  public int Page { get; set; }
  public int PageSize { get; set; }
  public int TotalCount { get; set; }
  public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

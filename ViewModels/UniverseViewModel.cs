using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class UniverseIndexViewModel
{
  public AppSettings Settings { get; set; } = new();
  public IReadOnlyList<UniverseMap> Maps { get; set; } = Array.Empty<UniverseMap>();
}

public class UniverseDetailViewModel
{
  public AppSettings Settings { get; set; } = new();
  public UniverseMap Map { get; set; } = new();
}

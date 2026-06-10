using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class ActorsIndexViewModel
{
  public AppSettings Settings { get; set; } = new();
  public IReadOnlyList<ActorSummary> Actors { get; set; } = Array.Empty<ActorSummary>();
}

public class ActorDetailViewModel
{
  public AppSettings Settings { get; set; } = new();
  public ActorProfile Actor { get; set; } = new();
}

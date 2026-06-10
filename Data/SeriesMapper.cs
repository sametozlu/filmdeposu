using FilmSerileri.Entities;
using FilmSerileri.Models;

namespace FilmSerileri.Data;

public static class SeriesMapper
{
  public static MovieSeries ToModel(SeriesEntity e) => new()
  {
    Id = e.Id,
    Title = e.Title,
    OriginalTitle = e.OriginalTitle,
    Tagline = e.Tagline,
    Description = e.Description,
    Genre = e.Genre,
    GenreKey = e.GenreKey,
    ReleaseYearStart = e.ReleaseYearStart,
    ReleaseYearEnd = e.ReleaseYearEnd,
    Director = e.Director,
    Studio = e.Studio,
    ImdbRating = e.ImdbRating,
    AccentColor = e.AccentColor,
    GradientFrom = e.GradientFrom,
    GradientTo = e.GradientTo,
    Icon = e.Icon,
    UniverseId = e.UniverseId,
    PosterUrl = PosterCatalog.Series(e.Id),
    BackdropUrl = PosterCatalog.Backdrop(e.Id),
    Movies = e.Movies.OrderBy(m => m.Order).Select(m => new Movie
    {
      Order = m.Order,
      Title = m.Title,
      Year = m.Year,
      DurationMinutes = m.DurationMinutes,
      Synopsis = m.Synopsis,
      ImdbRating = m.ImdbRating,
      PosterUrl = PosterCatalog.Movie(e.Id, m.Order)
    }).ToList(),
    Cast = e.Cast.Select(c => new CastMember
    {
      ActorName = c.ActorName,
      CharacterName = c.CharacterName,
      Role = c.Role,
      PhotoUrl = ActorPhotos.Get(c.ActorName)
    }).ToList()
  };

  public static SeriesEntity ToEntity(MovieSeries m, int sortOrder) => new()
  {
    Id = m.Id,
    Title = m.Title,
    OriginalTitle = m.OriginalTitle,
    Tagline = m.Tagline,
    Description = m.Description,
    Genre = m.Genre,
    GenreKey = m.GenreKey,
    ReleaseYearStart = m.ReleaseYearStart,
    ReleaseYearEnd = m.ReleaseYearEnd,
    Director = m.Director,
    Studio = m.Studio,
    ImdbRating = m.ImdbRating,
    AccentColor = m.AccentColor,
    GradientFrom = m.GradientFrom,
    GradientTo = m.GradientTo,
    Icon = m.Icon,
    UniverseId = m.UniverseId,
    SortOrder = sortOrder,
    Movies = m.Movies.Select(x => new SeriesMovieEntity
    {
      Order = x.Order,
      Title = x.Title,
      Year = x.Year,
      DurationMinutes = x.DurationMinutes,
      Synopsis = x.Synopsis,
      ImdbRating = x.ImdbRating
    }).ToList(),
    Cast = m.Cast.Select(x => new SeriesCastEntity
    {
      ActorName = x.ActorName,
      CharacterName = x.CharacterName,
      Role = x.Role
    }).ToList()
  };
}

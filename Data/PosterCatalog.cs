namespace FilmSerileri.Data;

public static class PosterCatalog
{
  public static string Series(string id) => $"/images/posters/{id}.jpg";
  public static string Movie(string seriesId, int order) => $"/images/posters/{seriesId}-{order}.jpg";
  public static string Backdrop(string seriesId) => $"/images/posters/{seriesId}.jpg";
}

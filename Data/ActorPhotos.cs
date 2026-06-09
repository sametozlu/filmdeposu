using System.Text.RegularExpressions;

namespace FilmSerileri.Data;

public static class ActorPhotos
{
  public static string Get(string name)
  {
    var slug = Slug(name);
    var localPath = Path.Combine("wwwroot", "images", "actors", $"{slug}.jpg");
    if (File.Exists(localPath))
      return $"/images/actors/{slug}.jpg";

    return AvatarUrl(name);
  }

  private static string Slug(string name)
  {
    var s = name.ToLowerInvariant();
    s = s.Replace("timothée", "timothee").Replace("'", "").Replace(".", "");
    s = Regex.Replace(s, @"[^a-z0-9]+", "-").Trim('-');
    return s;
  }

  private static string AvatarUrl(string name)
  {
    var encoded = Uri.EscapeDataString(name);
    return $"https://ui-avatars.com/api/?name={encoded}&background=2d2d44&color=fff&size=185&bold=true&format=png";
  }
}

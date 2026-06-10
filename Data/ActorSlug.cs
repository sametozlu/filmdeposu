using System.Text.RegularExpressions;

namespace FilmSerileri.Data;

public static class ActorSlug
{
  public static string FromName(string name)
  {
    var s = name.ToLowerInvariant();
    s = s.Replace("timothée", "timothee").Replace("'", "").Replace(".", "");
    s = Regex.Replace(s, @"[^a-z0-9]+", "-").Trim('-');
    return s;
  }
}

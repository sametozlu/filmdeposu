namespace FilmSerileri.Data;

/// <summary>Embed destekli YouTube Topic / resmi soundtrack videolari</summary>
public static class SeriesThemes
{
  public static string? GetYouTubeId(string seriesId) =>
    Themes.TryGetValue(seriesId, out var id) ? id : null;

  private static readonly Dictionary<string, string> Themes = new()
  {
    ["harry-potter"] = "wtHra9tFISY",          // Hedwig's Theme — John Williams
    ["yuzuklerin-efendisi"] = "IlmiRndxkU8",   // The Shire — Howard Shore
    ["alacakaranlik"] = "0jYqPEBRBnk",         // Bella's Lullaby — Carter Burwell
    ["labirent"] = "0n58DBOjDKg",              // The Maze Runner — John Paesano
    ["aclik-oyunlari"] = "PNnowhCZm5c",        // Horn of Plenty — James Newton Howard
    ["yildiz-savaslari"] = "e9lapdvLSGw",      // Main Title — John Williams
    ["karayip-korsanlari"] = "BuYf0taXoNw",    // He's a Pirate — Klaus Badelt
    ["jurassic-park"] = "AA_jatsA8JA",         // Theme from Jurassic Park — John Williams
    ["dune"] = "BdtiYwSP9ko",                  // Paul's Dream — Hans Zimmer
    ["matrix"] = "1yu-83mWkIU",                // Clubbed to Death — Rob Dougan
    ["kara-sovalye"] = "-L9M667JcaU",          // Why So Serious? — Hans Zimmer
    ["hizli-ve-ofkeli"] = "RgKAFK5djSk",       // See You Again — Wiz Khalifa ft. Charlie Puth
    ["orumcek-adam"] = "I42Y1bl5uXs",          // Spider-Man: Homecoming Suite — Michael Giacchino
    ["baba"] = "LaI5jYU-sPw",                  // The Godfather Waltz — Nino Rota
  };
}

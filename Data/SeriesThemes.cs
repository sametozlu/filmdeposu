namespace FilmSerileri.Data;

/// <summary>Embed destekli YouTube Topic / resmi soundtrack videolari</summary>
public static class SeriesThemes
{
  public static string? GetYouTubeId(string seriesId) =>
    Themes.TryGetValue(seriesId, out var id) ? id : null;

  private static readonly Dictionary<string, string> Themes = new()
  {
    ["harry-potter"] = "wtHra9tFISY",
    ["yuzuklerin-efendisi"] = "IlmiRndxkU8",
    ["alacakaranlik"] = "nIjVuRTm-dc",
    ["labirent"] = "0n58DBOjDKg",
    ["aclik-oyunlari"] = "k6M5C-oKw9k",
    ["yildiz-savaslari"] = "4JipHEz53sU",
    ["karayip-korsanlari"] = "BuYf0taXoNw",
    ["jurassic-park"] = "rDnO9ByOTE0",
    ["dune"] = "n9xhJrPXop4",
    ["matrix"] = "nUEQNVV3Gfs",
    ["kara-sovalye"] = "-L9M667JcaU",
    ["hizli-ve-ofkeli"] = "uelHwf8o7_U",
    ["orumcek-adam"] = "U9t-slLl30E",
    ["baba"] = "LaI5jYU-sPw",
  };
}

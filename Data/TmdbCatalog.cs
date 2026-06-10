namespace FilmSerileri.Data;

public static class TmdbCatalog
{
  public static readonly Dictionary<string, int> CollectionIds = new(StringComparer.OrdinalIgnoreCase)
  {
    ["harry-potter"] = 1241,
    ["yuzuklerin-efendisi"] = 119,
    ["alacakaranlik"] = 33514,
    ["labirent"] = 295130,
    ["aclik-oyunlari"] = 131635,
    ["yildiz-savaslari"] = 10,
    ["karayip-korsanlari"] = 295,
    ["jurassic-park"] = 328,
    ["dune"] = 726871,
    ["matrix"] = 2344,
    ["kara-sovalye"] = 263,
    ["hizli-ve-ofkeli"] = 9485,
    ["orumcek-adam"] = 531241,
    ["baba"] = 230,
  };

  public static readonly Dictionary<string, Dictionary<int, int>> MovieIds = new(StringComparer.OrdinalIgnoreCase)
  {
    ["harry-potter"] = new() { [1]=671, [2]=672, [3]=673, [4]=674, [5]=675, [6]=767, [7]=12444, [8]=12445 },
    ["yuzuklerin-efendisi"] = new() { [1]=120, [2]=121, [3]=122 },
    ["alacakaranlik"] = new() { [1]=8966, [2]=18239, [3]=24021, [4]=50620, [5]=7675 },
    ["labirent"] = new() { [1]=294963, [2]=294664, [3]=336843 },
    ["aclik-oyunlari"] = new() { [1]=70160, [2]=101299, [3]=131634, [4]=131631 },
    ["yildiz-savaslari"] = new() { [1]=11, [2]=1891, [3]=1892, [4]=1893, [5]=1894, [6]=1895, [7]=140607, [8]=181808, [9]=181812 },
    ["karayip-korsanlari"] = new() { [1]=22, [2]=58, [3]=285, [4]=1865, [5]=166426 },
    ["jurassic-park"] = new() { [1]=329, [2]=330, [3]=135397, [4]=351286, [5]=507086, [6]=507089 },
    ["dune"] = new() { [1]=438631, [2]=693134 },
    ["matrix"] = new() { [1]=603, [2]=604, [3]=605, [4]=624860 },
    ["kara-sovalye"] = new() { [1]=155, [2]=272, [3]=49026 },
    ["hizli-ve-ofkeli"] = new() { [1]=9799, [2]=584, [3]=9615, [4]=13804, [5]=51497, [6]=82992, [7]=168259 },
    ["orumcek-adam"] = new() { [1]=315635, [2]=429617, [3]=634649 },
    ["baba"] = new() { [1]=238, [2]=240, [3]=242 },
  };

  public static int? GetMovieId(string seriesId, int order) =>
    MovieIds.TryGetValue(seriesId, out var map) && map.TryGetValue(order, out var id) ? id : null;

  public static int? GetCollectionId(string seriesId) =>
    CollectionIds.TryGetValue(seriesId, out var id) ? id : null;
}

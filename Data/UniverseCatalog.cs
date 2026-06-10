using FilmSerileri.Models;

namespace FilmSerileri.Data;

public static class UniverseCatalog
{
  public static IReadOnlyList<UniverseMap> GetAll(string language) =>
    language == "en" ? MapsEn : MapsTr;

  public static UniverseMap? GetById(string id, string language) =>
    GetAll(language).FirstOrDefault(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

  private static readonly List<UniverseMap> MapsTr =
  [
    new()
    {
      Id = "star-wars",
      Title = "Star Wars Evreni",
      Description = "Skywalker saga ve genişletilmiş evren.",
      Nodes =
      [
        new() { Id = "ep1", Label = "Bölüm I", SeriesId = "yildiz-savaslari", X = 10, Y = 50, Color = "#ffd700" },
        new() { Id = "ep4", Label = "Bölüm IV", SeriesId = "yildiz-savaslari", X = 35, Y = 30, Color = "#ffd700" },
        new() { Id = "ep5", Label = "Bölüm V", SeriesId = "yildiz-savaslari", X = 55, Y = 50, Color = "#ffd700" },
        new() { Id = "ep7", Label = "Bölüm VII", SeriesId = "yildiz-savaslari", X = 75, Y = 35, Color = "#ffd700" },
        new() { Id = "ep9", Label = "Bölüm IX", SeriesId = "yildiz-savaslari", X = 90, Y = 55, Color = "#ffd700" },
      ]
    },
    new()
    {
      Id = "mcu-spider",
      Title = "MCU Örümcek-Adam",
      Description = "Marvel Sinematik Evreni içindeki Örümcek-Adam üçlemesi.",
      Nodes =
      [
        new() { Id = "sm1", Label = "Homecoming", SeriesId = "orumcek-adam", X = 20, Y = 50, Color = "#e23636" },
        new() { Id = "sm2", Label = "Far From Home", SeriesId = "orumcek-adam", X = 50, Y = 35, Color = "#e23636" },
        new() { Id = "sm3", Label = "No Way Home", SeriesId = "orumcek-adam", X = 80, Y = 50, Color = "#e23636" },
      ]
    },
    new()
    {
      Id = "middle-earth",
      Title = "Orta Dünya",
      Description = "Yüzüklerin Efendisi üçlemesi kronolojisi.",
      Nodes =
      [
        new() { Id = "fotr", Label = "Yüzük Kardeşliği", SeriesId = "yuzuklerin-efendisi", X = 25, Y = 50, Color = "#8b6914" },
        new() { Id = "ttt", Label = "İki Kule", SeriesId = "yuzuklerin-efendisi", X = 50, Y = 35, Color = "#8b6914" },
        new() { Id = "rotwk", Label = "Kralın Dönüşü", SeriesId = "yuzuklerin-efendisi", X = 75, Y = 50, Color = "#8b6914" },
      ]
    }
  ];

  private static readonly List<UniverseMap> MapsEn =
  [
    new()
    {
      Id = "star-wars",
      Title = "Star Wars Universe",
      Description = "Skywalker saga timeline across the galaxy.",
      Nodes =
      [
        new() { Id = "ep1", Label = "Episode I", SeriesId = "yildiz-savaslari", X = 10, Y = 50, Color = "#ffd700" },
        new() { Id = "ep4", Label = "Episode IV", SeriesId = "yildiz-savaslari", X = 35, Y = 30, Color = "#ffd700" },
        new() { Id = "ep5", Label = "Episode V", SeriesId = "yildiz-savaslari", X = 55, Y = 50, Color = "#ffd700" },
        new() { Id = "ep7", Label = "Episode VII", SeriesId = "yildiz-savaslari", X = 75, Y = 35, Color = "#ffd700" },
        new() { Id = "ep9", Label = "Episode IX", SeriesId = "yildiz-savaslari", X = 90, Y = 55, Color = "#ffd700" },
      ]
    },
    new()
    {
      Id = "mcu-spider",
      Title = "MCU Spider-Man",
      Description = "Tom Holland trilogy within the Marvel Cinematic Universe.",
      Nodes =
      [
        new() { Id = "sm1", Label = "Homecoming", SeriesId = "orumcek-adam", X = 20, Y = 50, Color = "#e23636" },
        new() { Id = "sm2", Label = "Far From Home", SeriesId = "orumcek-adam", X = 50, Y = 35, Color = "#e23636" },
        new() { Id = "sm3", Label = "No Way Home", SeriesId = "orumcek-adam", X = 80, Y = 50, Color = "#e23636" },
      ]
    },
    new()
    {
      Id = "middle-earth",
      Title = "Middle-earth",
      Description = "The Lord of the Rings trilogy chronology.",
      Nodes =
      [
        new() { Id = "fotr", Label = "Fellowship", SeriesId = "yuzuklerin-efendisi", X = 25, Y = 50, Color = "#8b6914" },
        new() { Id = "ttt", Label = "Two Towers", SeriesId = "yuzuklerin-efendisi", X = 50, Y = 35, Color = "#8b6914" },
        new() { Id = "rotwk", Label = "Return of the King", SeriesId = "yuzuklerin-efendisi", X = 75, Y = 50, Color = "#8b6914" },
      ]
    }
  ];
}

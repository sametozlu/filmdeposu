namespace FilmSerileri.Services;

public class LocalizationService : ILocalizationService
{
  private readonly ISettingsService _settingsService;

  private static readonly Dictionary<string, BilingualUi> Strings = new()
  {
    ["site_name"] = new("Sinema Serileri", "Cinema Series"),
    ["nav_home"] = new("Ana Sayfa", "Home"),
    ["nav_settings"] = new("Ayarlar", "Settings"),
    ["footer_tagline"] = new("Efsanevi film serilerini keşfedin.", "Discover legendary movie franchises."),
    ["featured_badge"] = new("Öne Çıkan Seri", "Featured Series"),
    ["explore_series"] = new("Seriyi Keşfet", "Explore Series"),
    ["all_series"] = new("Tüm Seriler", "All Series"),
    ["all_series_desc"] = new("En sevilen film serilerini, oyuncu kadrolarını ve film listelerini inceleyin.", "Browse beloved franchises, cast lists, and filmographies."),
    ["favorite"] = new("Favori", "Favorite"),
    ["films"] = new("film", "films"),
    ["stat_series"] = new("Film Serisi", "Franchises"),
    ["stat_movies"] = new("Toplam Film", "Total Films"),
    ["stat_cast"] = new("Oyuncu Profili", "Cast Profiles"),
    ["back_home"] = new("Ana Sayfa", "Home"),
    ["about"] = new("Hakkında", "About"),
    ["director"] = new("Yönetmen", "Director"),
    ["studio"] = new("Stüdyo", "Studio"),
    ["movie_count"] = new("Film Sayısı", "Film Count"),
    ["movies_section"] = new("Filmler", "Films"),
    ["cast_section"] = new("Oyuncu Kadrosu", "Cast"),
    ["theme_music"] = new("Film Müziği", "Theme Music"),
    ["theme_music_play"] = new("Müziği Başlat", "Play Theme"),
    ["theme_music_pause"] = new("Duraklat", "Pause"),
    ["theme_music_playing"] = new("Çalıyor", "Playing"),
    ["minutes"] = new("dk", "min"),
    ["settings_title"] = new("Ayarlar", "Settings"),
    ["settings_desc"] = new("Site deneyimini kişiselleştirin.", "Personalize your browsing experience."),
    ["settings_saved"] = new("Ayarlarınız kaydedildi!", "Your settings have been saved!"),
    ["appearance"] = new("Görünüm", "Appearance"),
    ["theme"] = new("Tema", "Theme"),
    ["theme_dark"] = new("Koyu Tema", "Dark Theme"),
    ["theme_light"] = new("Açık Tema", "Light Theme"),
    ["compact_view"] = new("Kompakt Görünüm", "Compact View"),
    ["show_ratings"] = new("IMDb Puanlarını Göster", "Show IMDb Ratings"),
    ["preferences"] = new("Tercihler", "Preferences"),
    ["favorite_series"] = new("Favori Seri", "Favorite Series"),
    ["favorite_hint"] = new("Favori seriniz ana sayfada ilk sırada gösterilir.", "Your favorite franchise appears first on the home page."),
    ["language"] = new("Dil", "Language"),
    ["lang_tr"] = new("Türkçe", "Turkish"),
    ["lang_en"] = new("English", "English"),
    ["select_none"] = new("Seçiniz (yok)", "None"),
    ["save"] = new("Kaydet", "Save"),
    ["search_placeholder"] = new("Seri, oyuncu veya tür ara...", "Search series, actor, or genre..."),
    ["filter_genre"] = new("Tür", "Genre"),
    ["filter_all_genres"] = new("Tüm Türler", "All Genres"),
    ["filter_min_rating"] = new("Min. Puan", "Min. Rating"),
    ["filter_any_rating"] = new("Tümü", "Any"),
    ["filter_sort"] = new("Sırala", "Sort"),
    ["sort_rating"] = new("Puana Göre", "By Rating"),
    ["sort_year"] = new("Yıla Göre", "By Year"),
    ["sort_title"] = new("İsme Göre", "By Title"),
    ["sort_movies"] = new("Film Sayısına Göre", "By Film Count"),
    ["search_results"] = new("sonuç bulundu", "results found"),
    ["no_results"] = new("Aramanızla eşleşen seri bulunamadı.", "No series match your search."),
    ["clear_filters"] = new("Filtreleri Temizle", "Clear Filters"),
    ["role_lead"] = new("Başrol", "Lead"),
    ["role_supporting"] = new("Yardımcı Rol", "Supporting"),
    ["role_antagonist"] = new("Antagonist", "Antagonist"),
    ["home_title"] = new("Ana Sayfa", "Home"),
  };

  public LocalizationService(ISettingsService settingsService)
  {
    _settingsService = settingsService;
  }

  public string CurrentLanguage => _settingsService.GetSettings().Language;

  public string T(string key, string? language = null)
  {
    var lang = language ?? CurrentLanguage;
    return Strings.TryGetValue(key, out var text) ? text.Get(lang) : key;
  }

  private record BilingualUi(string Tr, string En)
  {
    public string Get(string lang) =>
      lang.Equals("en", StringComparison.OrdinalIgnoreCase) ? En : Tr;
  }
}

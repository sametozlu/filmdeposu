using FilmSerileri.Data;
using FilmSerileri.Models;

namespace FilmSerileri.Services;

public class MovieService : IMovieService
{
  private readonly SeriesCatalogService _catalog;
  private readonly MovieEnrichmentService _enrichment;
  private readonly Dictionary<string, MovieSeries> _enrichedCache = new();

  public MovieService(MovieEnrichmentService enrichment, SeriesCatalogService catalog)
  {
    _enrichment = enrichment;
    _catalog = catalog;
  }

  private IReadOnlyList<MovieSeries> Source => _catalog.GetAll();

  public IReadOnlyList<MovieSeries> GetAllSeries(string language = "tr") =>
    SeriesLocalizer.LocalizeAll(Source, language).Select(_enrichment.Enrich).ToList();

  public MovieSeries? GetSeriesById(string id, string language = "tr")
  {
    var series = Source.FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    return series == null ? null : _enrichment.Enrich(SeriesLocalizer.Localize(series, language));
  }

  public void InvalidateEnrichedCache() => _enrichedCache.Clear();

  public async Task<MovieSeries?> GetSeriesByIdAsync(string id, string language = "tr", CancellationToken ct = default)
  {
    var cacheKey = $"{id}:{language}";
    if (_enrichedCache.TryGetValue(cacheKey, out var cached)) return cached;

    var series = GetSeriesById(id, language);
    if (series == null) return null;

    var enriched = await _enrichment.EnrichAsync(series, ct);
    _enrichedCache[cacheKey] = enriched;
    return enriched;
  }

  public MovieSeries? GetFeaturedSeries(string language = "tr")
  {
    var series = Source.FirstOrDefault(s => s.Id == "harry-potter") ?? Source.FirstOrDefault();
    return series == null ? null : SeriesLocalizer.Localize(series, language);
  }

  public IReadOnlyList<(string Key, string Label)> GetGenres(string language = "tr") =>
    GetAllSeries(language)
      .GroupBy(s => s.GenreKey)
      .Select(g => (Key: g.Key, Label: g.First().Genre))
      .OrderBy(g => g.Label)
      .ToList();

  public IReadOnlyList<MovieSeries> SearchSeries(string? query, string? genre, double? minRating, string sortBy, string language = "tr")
  {
    var results = GetAllSeries(language).AsEnumerable();

    if (!string.IsNullOrWhiteSpace(query))
    {
      var q = query.Trim().ToLowerInvariant();
      results = results.Where(s =>
        s.Title.ToLowerInvariant().Contains(q) ||
        s.OriginalTitle.ToLowerInvariant().Contains(q) ||
        s.Description.ToLowerInvariant().Contains(q) ||
        s.Genre.ToLowerInvariant().Contains(q) ||
        s.Director.ToLowerInvariant().Contains(q) ||
        s.Cast.Any(c => c.ActorName.ToLowerInvariant().Contains(q) || c.CharacterName.ToLowerInvariant().Contains(q)) ||
        s.Movies.Any(m => m.Title.ToLowerInvariant().Contains(q)));
    }

    if (!string.IsNullOrWhiteSpace(genre))
      results = results.Where(s => s.GenreKey == genre || s.Genre == genre);

    if (minRating.HasValue && minRating > 0)
      results = results.Where(s => s.ImdbRating >= minRating.Value);

    results = sortBy switch
    {
      "year" => results.OrderByDescending(s => s.ReleaseYearStart),
      "title" => results.OrderBy(s => s.Title),
      "movies" => results.OrderByDescending(s => s.Movies.Count),
      _ => results.OrderByDescending(s => s.ImdbRating)
    };

    return results.ToList();
  }

  public PagedResult<MovieSeries> SearchSeriesPaged(string? query, string? genre, double? minRating, string sortBy, int page, int pageSize, string language = "tr")
  {
    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 6, 24);
    var all = SearchSeries(query, genre, minRating, sortBy, language);
    return new PagedResult<MovieSeries>
    {
      Items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
      Page = page,
      PageSize = pageSize,
      TotalCount = all.Count
    };
  }

  public IReadOnlyList<MovieSeries> GetSimilarSeries(string id, string language = "tr")
  {
    var current = GetSeriesById(id, language);
    if (current == null) return Array.Empty<MovieSeries>();

    return GetAllSeries(language)
      .Where(s => s.Id != id && (s.GenreKey == current.GenreKey || Math.Abs(s.ReleaseYearStart - current.ReleaseYearStart) <= 5))
      .OrderByDescending(s => s.GenreKey == current.GenreKey)
      .ThenByDescending(s => s.ImdbRating)
      .Take(4)
      .ToList();
  }

  public MovieSeries GetRandomSeries(string language = "tr")
  {
    var all = GetAllSeries(language);
    return all[Random.Shared.Next(all.Count)];
  }

  public ActorProfile? GetActorBySlug(string slug, string language = "tr")
  {
    foreach (var series in GetAllSeries(language))
    {
      foreach (var member in series.Cast)
      {
        if (ActorSlug.FromName(member.ActorName) != slug) continue;
        return BuildActorProfile(member, language);
      }
    }
    return null;
  }

  public async Task<ActorProfile?> GetActorBySlugAsync(string slug, string language = "tr", CancellationToken ct = default)
  {
    var profile = GetActorBySlug(slug, language);
    return profile == null ? null : await _enrichment.EnrichActorAsync(profile, ct);
  }

  public IReadOnlyList<ActorSummary> GetAllActors(string language = "tr")
  {
    return GetAllSeries(language)
      .SelectMany(s => s.Cast.Select(c => new { c.ActorName, c.PhotoUrl }))
      .GroupBy(a => a.ActorName, StringComparer.OrdinalIgnoreCase)
      .Select(g => new ActorSummary
      {
        Slug = ActorSlug.FromName(g.Key),
        Name = g.Key,
        PhotoUrl = g.First().PhotoUrl
      })
      .OrderBy(a => a.Name)
      .ToList();
  }

  public SeriesComparison? CompareSeries(string idA, string idB, string language = "tr")
  {
    var a = GetSeriesById(idA, language);
    var b = GetSeriesById(idB, language);
    if (a == null || b == null) return null;
    return new SeriesComparison { SeriesA = a, SeriesB = b };
  }

  public async Task WarmupCacheAsync(CancellationToken ct = default)
  {
    foreach (var series in Source)
      await GetSeriesByIdAsync(series.Id, "tr", ct);
  }

  private ActorProfile BuildActorProfile(CastMember member, string language)
  {
    var roles = GetAllSeries(language)
      .SelectMany(s => s.Cast.Where(c => c.ActorName.Equals(member.ActorName, StringComparison.OrdinalIgnoreCase))
        .Select(c => new ActorRole
        {
          SeriesId = s.Id,
          SeriesTitle = s.Title,
          CharacterName = c.CharacterName,
          Role = c.Role,
          PosterUrl = s.PosterUrl
        }))
      .ToList();

    ActorTmdbIds.Ids.TryGetValue(member.ActorName, out var tmdbId);

    return new ActorProfile
    {
      Slug = ActorSlug.FromName(member.ActorName),
      Name = member.ActorName,
      PhotoUrl = member.PhotoUrl,
      TmdbId = tmdbId,
      Roles = roles
    };
  }

  internal static List<MovieSeries> BuildSeries() =>
  [
    BuildHarryPotter(),
    BuildLotr(),
    BuildTwilight(),
    BuildMazeRunner(),
    BuildHungerGames(),
    BuildStarWars(),
    BuildPirates(),
    BuildJurassicPark(),
    BuildDune(),
    BuildMatrix(),
    BuildBatman(),
    BuildFastFurious(),
    BuildSpiderMan(),
    BuildGodfather()
  ];

  private static MovieSeries BuildHarryPotter() => new()
  {
    Id = "harry-potter",
    Title = "Harry Potter",
    OriginalTitle = "Harry Potter",
    Tagline = "Büyü seni bekliyor.",
    Description = "J.K. Rowling'in efsanevi kitap serisinden uyarlanan Harry Potter, genç bir büyücünün Hogwarts Cadılık ve Büyücülük Okulu'ndaki maceralarını anlatır. Dostluk, cesaret ve karanlığa karşı mücadele temalarıyla nesiller boyu izleyiciyi büyülemiştir.",
    Genre = "Fantastik / Macera",
    GenreKey = "fantasy",
    ReleaseYearStart = 2001,
    ReleaseYearEnd = 2011,
    Director = "Chris Columbus, Alfonso Cuarón, Mike Newell, David Yates",
    Studio = "Warner Bros.",
    ImdbRating = 7.6,
    AccentColor = "#d4a853",
    GradientFrom = "#1a0f2e",
    GradientTo = "#2d1b4e",
    Icon = "⚡",
    PosterUrl = PosterCatalog.Series("harry-potter"),
    BackdropUrl = PosterCatalog.Backdrop("harry-potter"),
    Movies = MoviesFrom("harry-potter",
    [
      (1, "Harry Potter ve Felsefe Taşı", 2001, 152, 7.7, "Harry Potter, büyücü dünyasını keşfeder ve gizemli Felsefe Taşı'nı korur."),
      (2, "Harry Potter ve Sırlar Odası", 2002, 161, 7.5, "Hogwarts'ta öğrenciler taşlaşmaya başlar; Sırlar Odası'nın sırrı çözülmelidir."),
      (3, "Harry Potter ve Azkaban Tutsağı", 2004, 142, 7.9, "Kaçak mahkum Sirius Black, Harry'nin geçmişiyle bağlantılıdır."),
      (4, "Harry Potter ve Ateş Kadehi", 2005, 157, 7.7, "Üçbüyü Turnuvası ve Voldemort'un geri dönüşü."),
      (5, "Harry Potter ve Zümrüdüanka Yoldaşlığı", 2007, 138, 7.5, "Harry, Voldemort'a karşı direniş örgütü kurar."),
      (6, "Harry Potter ve Melez Prens", 2009, 153, 7.6, "Voldemort'un geçmişi ve Horcrux'ların sırrı ortaya çıkar."),
      (7, "Harry Potter ve Ölüm Yadigârları: Bölüm 1", 2010, 146, 7.7, "Harry, Ron ve Hermione Horcrux avına çıkar."),
      (8, "Harry Potter ve Ölüm Yadigârları: Bölüm 2", 2011, 130, 8.1, "Hogwarts'ta son savaş: Harry ve Voldemort yüzleşir.")
    ]),
    Cast = Cast(
      ("Daniel Radcliffe", "Harry Potter", "Başrol"),
      ("Emma Watson", "Hermione Granger", "Başrol"),
      ("Rupert Grint", "Ron Weasley", "Başrol"),
      ("Alan Rickman", "Severus Snape", "Yardımcı Rol"),
      ("Ralph Fiennes", "Lord Voldemort", "Antagonist"),
      ("Maggie Smith", "Minerva McGonagall", "Yardımcı Rol"))
  };

  private static MovieSeries BuildLotr() => new()
  {
    Id = "yuzuklerin-efendisi",
    Title = "Yüzüklerin Efendisi",
    OriginalTitle = "The Lord of the Rings",
    Tagline = "Tek Yüzük hepsini yönetecek.",
    Description = "J.R.R. Tolkien'in Orta Dünya destanı, hobbit Frodo Baggins'in Karanlık Lord Sauron'u durdurmak için Tek Yüzük'ü yok etme yolculuğunu anlatır. Epik savaşlar, derin karakterler ve görsel şölen ile sinema tarihinin en büyük üçlemelerinden biridir.",
    Genre = "Fantastik / Epik Macera",
    GenreKey = "fantasy",
    ReleaseYearStart = 2001,
    ReleaseYearEnd = 2003,
    Director = "Peter Jackson",
    Studio = "New Line Cinema",
    ImdbRating = 8.8,
    AccentColor = "#c9a227",
    GradientFrom = "#0d1f0d",
    GradientTo = "#1a3a1a",
    Icon = "💍",
    PosterUrl = PosterCatalog.Series("yuzuklerin-efendisi"),
    BackdropUrl = PosterCatalog.Backdrop("yuzuklerin-efendisi"),
    Movies = MoviesFrom("yuzuklerin-efendisi",
    [
      (1, "Yüzüklerin Efendisi: Yüzük Kardeşliği", 2001, 178, 8.8, "Frodo ve Yüzük Kardeşliği, Yüzük'ü Mordor'a götürmek için yola çıkar."),
      (2, "Yüzüklerin Efendisi: İki Kule", 2002, 179, 8.8, "Kardeşlik dağılır; savaşlar ve ihanetler Orta Dünya'yı sarar."),
      (3, "Yüzüklerin Efendisi: Kralın Dönüşü", 2003, 201, 9.0, "Son savaş: Gondor'un savunması ve Yüzük'ün yok edilişi.")
    ]),
    Cast = Cast(
      ("Elijah Wood", "Frodo Baggins", "Başrol"),
      ("Viggo Mortensen", "Aragorn", "Başrol"),
      ("Ian McKellen", "Gandalf", "Başrol"),
      ("Sean Astin", "Samwise Gamgee", "Başrol"),
      ("Orlando Bloom", "Legolas", "Yardımcı Rol"),
      ("Andy Serkis", "Gollum", "Yardımcı Rol"))
  };

  private static MovieSeries BuildTwilight() => new()
  {
    Id = "alacakaranlik",
    Title = "Alacakaranlık",
    OriginalTitle = "Twilight",
    Tagline = "Aşk ölümsüzdür.",
    Description = "Stephenie Meyer'in romantik fantastik serisi, insan Bella Swan ile vampir Edward Cullen arasındaki yasak aşkı konu alır. Washington'un yağmurlu Forks kasabasında geçen hikâye, gençlik ve aşk temalarını doğaüstü öğelerle birleştirir.",
    Genre = "Romantik / Fantastik",
    GenreKey = "romance",
    ReleaseYearStart = 2008,
    ReleaseYearEnd = 2012,
    Director = "Catherine Hardwicke, Chris Weitz, David Slade, Bill Condon",
    Studio = "Summit Entertainment",
    ImdbRating = 5.3,
    AccentColor = "#8b0000",
    GradientFrom = "#1a0a0a",
    GradientTo = "#3d1515",
    Icon = "🌙",
    PosterUrl = PosterCatalog.Series("alacakaranlik"),
    BackdropUrl = PosterCatalog.Backdrop("alacakaranlik"),
    Movies = MoviesFrom("alacakaranlik",
    [
      (1, "Alacakaranlık", 2008, 122, 5.3, "Bella, gizemli Edward ile tanışır ve onun bir vampir olduğunu öğrenir."),
      (2, "Alacakaranlık Efsanesi: Yeni Ay", 2009, 130, 4.8, "Edward ayrılır; Bella Jacob ile yakınlaşır ve kurt adamları keşfeder."),
      (3, "Alacakaranlık Efsanesi: Tutulma", 2010, 124, 5.0, "Victoria'nın intikamı ve Edward ile Bella'nın yeniden bir araya gelişi."),
      (4, "Alacakaranlık Efsanesi: Şafak Vakti – Bölüm 1", 2011, 117, 5.0, "Bella ve Edward evlenir; hamilelik beklenmedik tehlikeler getirir."),
      (5, "Alacakaranlık Efsanesi: Şafak Vakti – Bölüm 2", 2012, 115, 5.5, "Volturi ile son yüzleşme ve Bella'nın vampir olma yolculuğu tamamlanır.")
    ]),
    Cast = Cast(
      ("Kristen Stewart", "Bella Swan", "Başrol"),
      ("Robert Pattinson", "Edward Cullen", "Başrol"),
      ("Taylor Lautner", "Jacob Black", "Başrol"),
      ("Ashley Greene", "Alice Cullen", "Yardımcı Rol"),
      ("Peter Facinelli", "Carlisle Cullen", "Yardımcı Rol"),
      ("Nikki Reed", "Rosalie Hale", "Yardımcı Rol"))
  };

  private static MovieSeries BuildMazeRunner() => new()
  {
    Id = "labirent",
    Title = "Labirent",
    OriginalTitle = "The Maze Runner",
    Tagline = "Kaçış tek seçenek.",
    Description = "James Dashner'ın distopik gençlik romanından uyarlanan seri, hafızasını kaybetmiş gençlerin dev bir labirentin ortasındaki Glade adlı toplulukta hayatta kalma mücadelesini anlatır. Gizem, aksiyon ve sürprizlerle dolu bir macera.",
    Genre = "Distopya / Bilim Kurgu / Aksiyon",
    GenreKey = "scifi",
    ReleaseYearStart = 2014,
    ReleaseYearEnd = 2018,
    Director = "Wes Ball",
    Studio = "20th Century Fox",
    ImdbRating = 6.8,
    AccentColor = "#2ecc71",
    GradientFrom = "#0a1a14",
    GradientTo = "#143d2e",
    Icon = "🧩",
    PosterUrl = PosterCatalog.Series("labirent"),
    BackdropUrl = PosterCatalog.Backdrop("labirent"),
    Movies = MoviesFrom("labirent",
    [
      (1, "Labirent: Ölümcül Kaçış", 2014, 113, 6.8, "Thomas uyanır ve kendini dev labirentin ortasında bulur."),
      (2, "Labirent: Alev Deneyleri", 2015, 132, 6.3, "Hayatta kalanlar çorak topraklarda WCKD'ye karşı savaşır."),
      (3, "Labirent: Ölümcül İlaç", 2018, 143, 5.4, "Son şans: antidotu bulmak ve WCKD'yi durdurmak.")
    ]),
    Cast = Cast(
      ("Dylan O'Brien", "Thomas", "Başrol"),
      ("Kaya Scodelario", "Teresa Agnes", "Başrol"),
      ("Thomas Brodie-Sangster", "Newt", "Yardımcı Rol"),
      ("Ki Hong Lee", "Minho", "Yardımcı Rol"),
      ("Will Poulter", "Gally", "Yardımcı Rol"),
      ("Patricia Clarkson", "Ava Paige", "Antagonist"))
  };

  private static MovieSeries BuildHungerGames() => new()
  {
    Id = "aclik-oyunlari",
    Title = "Açlık Oyunları",
    OriginalTitle = "The Hunger Games",
    Tagline = "Hayatta kalmak için oyna.",
    Description = "Suzanne Collins'in distopik romanından uyarlanan seri, totaliter Panem ülkesinde gençlerin televizyonda yayınlanan ölümcül bir turnuvada savaşmasını konu alır. Katniss Everdeen'in cesareti ve direnişi, umut ve adalet temalarını öne çıkarır.",
    Genre = "Distopya / Bilim Kurgu / Aksiyon",
    GenreKey = "scifi",
    ReleaseYearStart = 2012,
    ReleaseYearEnd = 2015,
    Director = "Gary Ross, Francis Lawrence",
    Studio = "Lionsgate",
    ImdbRating = 7.2,
    AccentColor = "#e74c3c",
    GradientFrom = "#1a0f0a",
    GradientTo = "#3d1f14",
    Icon = "🏹",
    PosterUrl = PosterCatalog.Series("aclik-oyunlari"),
    BackdropUrl = PosterCatalog.Backdrop("aclik-oyunlari"),
    Movies = MoviesFrom("aclik-oyunlari",
    [
      (1, "Açlık Oyunları", 2012, 142, 7.2, "Katniss, kız kardeşi yerine Açlık Oyunları'na katılır."),
      (2, "Açlık Oyunları: Ateşi Yakalamak", 2013, 146, 7.5, "Zafer turu ve isyanın kıvılcımı."),
      (3, "Açlık Oyunları: Alaycı Kuş – Bölüm 1", 2014, 123, 6.7, "Katniss Mockingjay sembolü olur; Capitol'a savaş ilan edilir."),
      (4, "Açlık Oyunları: Alaycı Kuş – Bölüm 2", 2015, 137, 6.9, "Capitol'a son saldırı ve Snow'un düşüşü.")
    ]),
    Cast = Cast(
      ("Jennifer Lawrence", "Katniss Everdeen", "Başrol"),
      ("Josh Hutcherson", "Peeta Mellark", "Başrol"),
      ("Liam Hemsworth", "Gale Hawthorne", "Başrol"),
      ("Woody Harrelson", "Haymitch Abernathy", "Yardımcı Rol"),
      ("Elizabeth Banks", "Effie Trinket", "Yardımcı Rol"),
      ("Donald Sutherland", "Başkan Snow", "Antagonist"))
  };

  private static MovieSeries BuildStarWars() => new()
  {
    Id = "yildiz-savaslari",
    Title = "Yıldız Savaşları",
    OriginalTitle = "Star Wars",
    Tagline = "Çok uzaklarda bir galaksi...",
    Description = "George Lucas'ın uzay operası, nesiller boyu Jedi'lar, Sith'ler ve asilerin mücadelesini anlatır. Işık kılıçları, ikonik kahramanlar ve iyilikle kötülük arasındaki epik savaş modern blockbuster sinemasını şekillendirdi.",
    Genre = "Bilim Kurgu / Uzay Operası / Macera",
    GenreKey = "scifi",
    ReleaseYearStart = 1999,
    ReleaseYearEnd = 2019,
    Director = "George Lucas, J.J. Abrams, Rian Johnson",
    Studio = "Lucasfilm / Disney",
    ImdbRating = 7.5,
    AccentColor = "#ffd700",
    GradientFrom = "#0a0a1a",
    GradientTo = "#1a1a3a",
    Icon = "⭐",
    PosterUrl = PosterCatalog.Series("yildiz-savaslari"),
    BackdropUrl = PosterCatalog.Backdrop("yildiz-savaslari"),
    Movies = MoviesFrom("yildiz-savaslari",
    [
      (1, "Bölüm I: Gizli Tehlike", 1999, 136, 6.5, "Genç Anakin Skywalker ve Naboo'nun işgali."),
      (2, "Bölüm II: Klon Savaşları", 2002, 142, 6.6, "Klon Savaşları başlar; Anakin ve Padmé aşık olur."),
      (3, "Bölüm III: Sith'in İntikamı", 2005, 140, 7.6, "Anakin karanlık tarafa geçer; İmparatorluk yükselir."),
      (4, "Bölüm IV: Yeni Bir Umut", 1977, 121, 8.6, "Luke Skywalker, İmparatorluğa karşı İsyan'a katılır."),
      (5, "Bölüm V: İmparator'un Dönüşü", 1980, 124, 8.7, "Luke Yoda ile eğitim alır; Vader'ın şok edici itirafı."),
      (6, "Bölüm VI: Jedi'nin Dönüşü", 1983, 131, 8.3, "İsyan'ın İmparatorluğa son darbesi."),
      (7, "Bölüm VII: Güç Uyanıyor", 2015, 138, 7.8, "Yeni nesil, yükselen Birinci Düzen'e karşı durur."),
      (8, "Bölüm VIII: Son Jedi", 2017, 152, 6.9, "Rey Luke'u arar; Direniş hayatta kalmak için savaşır."),
      (9, "Bölüm IX: Skywalker'ın Yükselişi", 2019, 142, 6.5, "İmparator Palpatine'e karşı son savaş.")
    ]),
    Cast = Cast(
      ("Mark Hamill", "Luke Skywalker", "Başrol"),
      ("Harrison Ford", "Han Solo", "Başrol"),
      ("Carrie Fisher", "Leia Organa", "Başrol"),
      ("Daisy Ridley", "Rey", "Başrol"),
      ("Adam Driver", "Kylo Ren", "Antagonist"),
      ("Ewan McGregor", "Obi-Wan Kenobi", "Yardımcı Rol"))
  };

  private static MovieSeries BuildPirates() => new()
  {
    Id = "karayip-korsanlari",
    Title = "Karayip Korsanları",
    OriginalTitle = "Pirates of the Caribbean",
    Tagline = "Nereye gitmek istersek, oraya gideriz.",
    Description = "Disney'in korsan macerası, eksantrik Kaptan Jack Sparrow ve mürettebatını lanetli hazineler, deniz canavarları ve doğaüstü düşmanlarla dolu açık denizlerde takip eder.",
    Genre = "Macera / Fantastik / Aksiyon",
    GenreKey = "adventure",
    ReleaseYearStart = 2003,
    ReleaseYearEnd = 2017,
    Director = "Gore Verbinski, Rob Marshall, Joachim Rønning",
    Studio = "Disney",
    ImdbRating = 7.0,
    AccentColor = "#8B4513",
    GradientFrom = "#1a1008",
    GradientTo = "#3d2810",
    Icon = "🏴‍☠️",
    PosterUrl = PosterCatalog.Series("karayip-korsanlari"),
    BackdropUrl = PosterCatalog.Backdrop("karayip-korsanlari"),
    Movies = MoviesFrom("karayip-korsanlari",
    [
      (1, "Siyah İnci'nin Laneti", 2003, 143, 8.1, "Jack Sparrow ve Will Turner lanetli Aztek altınını arar."),
      (2, "Ölü Adamın Sandığı", 2006, 151, 7.4, "Jack'in ruhu Davy Jones'a borçludur."),
      (3, "Dünyanın Sonu", 2007, 169, 7.1, "Korsanlar Doğu Hindistan Şirketi'ne karşı birleşir."),
      (4, "Gizemli Denizlerde", 2011, 136, 6.6, "Gençlik Pınarı arayışı."),
      (5, "Ölü Adamın Hikâyeleri", 2017, 129, 6.5, "Jack, hayalet Kaptan Salazar ile yüzleşir.")
    ]),
    Cast = Cast(
      ("Johnny Depp", "Jack Sparrow", "Başrol"),
      ("Orlando Bloom", "Will Turner", "Başrol"),
      ("Keira Knightley", "Elizabeth Swann", "Başrol"),
      ("Geoffrey Rush", "Hector Barbossa", "Yardımcı Rol"),
      ("Bill Nighy", "Davy Jones", "Antagonist"),
      ("Javier Bardem", "Captain Salazar", "Antagonist"))
  };

  private static MovieSeries BuildJurassicPark() => new()
  {
    Id = "jurassic-park",
    Title = "Jurassic Park",
    OriginalTitle = "Jurassic Park",
    Tagline = "Yaşam bir yolunu bulur.",
    Description = "Steven Spielberg'in çığır açan serisi, klonlama yoluyla dinozorları hayata döndürür. Kontrolden çıkan bilim, nefes kesen kovalamacalar ve tarih öncesi yaratıkların büyüsü izleyicileri büyüler.",
    Genre = "Bilim Kurgu / Macera / Gerilim",
    GenreKey = "scifi",
    ReleaseYearStart = 1993,
    ReleaseYearEnd = 2022,
    Director = "Steven Spielberg, Colin Trevorrow",
    Studio = "Universal Pictures",
    ImdbRating = 7.0,
    AccentColor = "#228B22",
    GradientFrom = "#0a1a0a",
    GradientTo = "#1a3d1a",
    Icon = "🦖",
    PosterUrl = PosterCatalog.Series("jurassic-park"),
    BackdropUrl = PosterCatalog.Backdrop("jurassic-park"),
    Movies = MoviesFrom("jurassic-park",
    [
      (1, "Jurassic Park", 1993, 127, 8.2, "Dinozorlar bir tema parkında dolaşır; sistemler çöker."),
      (2, "Kayıp Dünya", 1997, 129, 6.5, "İkinci bir dinozor adası keşfedilir."),
      (3, "Jurassic Park III", 2001, 92, 5.9, "Isla Sorna'daki kurtarma görevi ters gider."),
      (4, "Jurassic World", 2015, 124, 6.9, "Genetik olarak değiştirilmiş bir hibritle yeni park açılır."),
      (5, "Yıkılmış Krallık", 2018, 128, 6.1, "Ada volkanı patladıktan sonra dinozorlar yok olma tehlikesiyle karşı karşıya."),
      (6, "Hakimiyet", 2022, 147, 5.6, "Dinozorlar artık dünya genelinde insanlarla yaşıyor.")
    ]),
    Cast = Cast(
      ("Sam Neill", "Dr. Alan Grant", "Başrol"),
      ("Laura Dern", "Dr. Ellie Sattler", "Başrol"),
      ("Jeff Goldblum", "Dr. Ian Malcolm", "Başrol"),
      ("Chris Pratt", "Owen Grady", "Başrol"),
      ("Bryce Dallas Howard", "Claire Dearing", "Başrol"),
      ("Richard Attenborough", "John Hammond", "Yardımcı Rol"))
  };

  private static MovieSeries BuildDune() => new()
  {
    Id = "dune",
    Title = "Dune",
    OriginalTitle = "Dune",
    Tagline = "Baharatı kontrol eden evreni kontrol eder.",
    Description = "Frank Herbert'in bilim kurgu destanı, evrendeki en değerli maddeye ev sahipliği yapan çöl gezegeni Arrakis'te Paul Atreides'in hikâyesini anlatır. Politika, kehanet ve kum solucanları büyük bir destanı şekillendirir.",
    Genre = "Bilim Kurgu / Epik / Macera",
    GenreKey = "scifi",
    ReleaseYearStart = 2021,
    ReleaseYearEnd = 2024,
    Director = "Denis Villeneuve",
    Studio = "Legendary / Warner Bros.",
    ImdbRating = 8.0,
    AccentColor = "#c2a366",
    GradientFrom = "#1a1408",
    GradientTo = "#3d3010",
    Icon = "🏜️",
    PosterUrl = PosterCatalog.Series("dune"),
    BackdropUrl = PosterCatalog.Backdrop("dune"),
    Movies = MoviesFrom("dune",
    [
      (1, "Dune: Birinci Bölüm", 2021, 155, 8.0, "Paul Atreides Arrakis'e gelir; Atreides Hanedanı ihanete uğrar."),
      (2, "Dune: İkinci Bölüm", 2024, 166, 8.5, "Paul Fremen'leri birleştirir ve İmparator'a meydan okur.")
    ]),
    Cast = Cast(
      ("Timothée Chalamet", "Paul Atreides", "Başrol"),
      ("Zendaya", "Chani", "Başrol"),
      ("Rebecca Ferguson", "Lady Jessica", "Başrol"),
      ("Oscar Isaac", "Duke Leto", "Yardımcı Rol"),
      ("Josh Brolin", "Gurney Halleck", "Yardımcı Rol"),
      ("Javier Bardem", "Stilgar", "Yardımcı Rol"))
  };

  private static MovieSeries BuildMatrix() => new()
  {
    Id = "matrix",
    Title = "Matrix",
    OriginalTitle = "The Matrix",
    Tagline = "Hoş geldin gerçeğin dünyasına.",
    Description = "Wachowski kardeşlerin kült bilim kurgu serisi, insanlığın makineler tarafından simüle edilmiş bir gerçeklikte hapsedildiği distopik bir geleceği anlatır. Neo'nun uyanışı aksiyon ve felsefeyi birleştirir.",
    Genre = "Bilim Kurgu / Aksiyon",
    GenreKey = "scifi",
    ReleaseYearStart = 1999,
    ReleaseYearEnd = 2021,
    Director = "Lana ve Lilly Wachowski",
    Studio = "Warner Bros.",
    ImdbRating = 7.4,
    AccentColor = "#00ff41",
    GradientFrom = "#0a1a0a",
    GradientTo = "#0a2a14",
    Icon = "💊",
    PosterUrl = PosterCatalog.Series("matrix"),
    BackdropUrl = PosterCatalog.Backdrop("matrix"),
    Movies = MoviesFrom("matrix",
    [
      (1, "Matrix", 1999, 136, 8.7, "Neo, gerçek dünyanın korkunç sırrını öğrenir."),
      (2, "Matrix Reloaded", 2003, 138, 7.2, "Zion tehdit altında; Neo güçlerini keşfeder."),
      (3, "Matrix Revolutions", 2003, 129, 6.8, "İnsanlık ve makineler arasında son savaş."),
      (4, "Matrix Resurrections", 2021, 148, 5.7, "Neo tekrar Matrix'e döner; eski ve yeni gerçeklikler çarpışır.")
    ]),
    Cast = Cast(
      ("Keanu Reeves", "Neo", "Başrol"),
      ("Laurence Fishburne", "Morpheus", "Başrol"),
      ("Carrie-Anne Moss", "Trinity", "Başrol"),
      ("Hugo Weaving", "Agent Smith", "Antagonist"),
      ("Jada Pinkett Smith", "Niobe", "Yardımcı Rol"),
      ("Lambert Wilson", "The Merovingian", "Yardımcı Rol"))
  };

  private static MovieSeries BuildBatman() => new()
  {
    Id = "kara-sovalye",
    Title = "Kara Şövalye",
    OriginalTitle = "The Dark Knight Trilogy",
    Tagline = "Karanlık yükselir.",
    Description = "Christopher Nolan'ın Batman üçlemesi, Bruce Wayne'in Gotham'ı kurtarma mücadelesini gerçekçi ve epik bir tonla anlatır. Kahramanlık, terör ve adalet temalarını derinlemesine işler.",
    Genre = "Aksiyon / Dram / Süper Kahraman",
    GenreKey = "action",
    ReleaseYearStart = 2005,
    ReleaseYearEnd = 2012,
    Director = "Christopher Nolan",
    Studio = "Warner Bros. / Legendary",
    ImdbRating = 8.3,
    AccentColor = "#1a1a1a",
    GradientFrom = "#0a0a0a",
    GradientTo = "#1a1a2e",
    Icon = "🦇",
    PosterUrl = PosterCatalog.Series("kara-sovalye"),
    BackdropUrl = PosterCatalog.Backdrop("kara-sovalye"),
    Movies = MoviesFrom("kara-sovalye",
    [
      (1, "Batman Begins", 2005, 140, 8.2, "Bruce Wayne Batman olur ve Gotham'ı korumaya başlar."),
      (2, "The Dark Knight", 2008, 152, 9.0, "Joker Gotham'ı kaosa sürükler."),
      (3, "The Dark Knight Rises", 2012, 164, 8.4, "Bane Gotham'ı kuşatır; Batman geri döner.")
    ]),
    Cast = Cast(
      ("Christian Bale", "Bruce Wayne / Batman", "Başrol"),
      ("Heath Ledger", "Joker", "Antagonist"),
      ("Aaron Eckhart", "Harvey Dent", "Başrol"),
      ("Michael Caine", "Alfred", "Yardımcı Rol"),
      ("Gary Oldman", "Jim Gordon", "Yardımcı Rol"),
      ("Tom Hardy", "Bane", "Antagonist"))
  };

  private static MovieSeries BuildFastFurious() => new()
  {
    Id = "hizli-ve-ofkeli",
    Title = "Hızlı ve Öfkeli",
    OriginalTitle = "Fast & Furious",
    Tagline = "Aile her şeyden önce gelir.",
    Description = "Sokak yarışından küresel casusluğa uzanan aksiyon serisi, Dominic Toretto ve ailesinin sadakat, hız ve adrenalin dolu maceralarını anlatır.",
    Genre = "Aksiyon / Macera",
    GenreKey = "action",
    ReleaseYearStart = 2001,
    ReleaseYearEnd = 2023,
    Director = "Rob Cohen, Justin Lin, Louis Leterrier",
    Studio = "Universal Pictures",
    ImdbRating = 6.8,
    AccentColor = "#ff4500",
    GradientFrom = "#1a0a00",
    GradientTo = "#3d1a00",
    Icon = "🏎️",
    PosterUrl = PosterCatalog.Series("hizli-ve-ofkeli"),
    BackdropUrl = PosterCatalog.Backdrop("hizli-ve-ofkeli"),
    Movies = MoviesFrom("hizli-ve-ofkeli",
    [
      (1, "Hızlı ve Öfkeli", 2001, 106, 6.8, "Gizli polis sokak yarışçıları çetesine sızar."),
      (2, "2 Fast 2 Furious", 2003, 107, 5.9, "Brian ve Roman Miami'de görev alır."),
      (3, "Tokyo Drift", 2006, 104, 6.0, "Tokyo'da drift yarışları ve yakuza."),
      (4, "Fast & Furious", 2009, 107, 6.6, "Dom ve Brian yeniden bir araya gelir."),
      (5, "Fast Five", 2011, 130, 7.3, "Rio'da büyük soygun ve aile kurulur."),
      (6, "Fast & Furious 6", 2013, 130, 7.0, "Ekibin karşısında yeni bir düşman."),
      (7, "Furious 7", 2015, 137, 7.1, "Deckard Shaw intikam peşinde.")
    ]),
    Cast = Cast(
      ("Vin Diesel", "Dominic Toretto", "Başrol"),
      ("Paul Walker", "Brian O'Conner", "Başrol"),
      ("Michelle Rodriguez", "Letty Ortiz", "Başrol"),
      ("Dwayne Johnson", "Luke Hobbs", "Yardımcı Rol"),
      ("Jason Statham", "Deckard Shaw", "Antagonist"),
      ("Jordana Brewster", "Mia Toretto", "Yardımcı Rol"))
  };

  private static MovieSeries BuildSpiderMan() => new()
  {
    Id = "orumcek-adam",
    Title = "Örümcek Adam",
    OriginalTitle = "Spider-Man (MCU)",
    Tagline = "Büyük güç, büyük sorumluluk getirir.",
    Description = "Marvel Sinematografik Evreni'nde Tom Holland'ın canlandırdığı genç Peter Parker, Tony Stark'ın mentörlüğünde Örümcek Adam olur ve çoklu evren maceralarına atılır.",
    Genre = "Aksiyon / Süper Kahraman / Macera",
    GenreKey = "action",
    ReleaseYearStart = 2017,
    ReleaseYearEnd = 2021,
    Director = "Jon Watts",
    Studio = "Marvel / Sony",
    ImdbRating = 7.4,
    AccentColor = "#e23636",
    GradientFrom = "#1a0000",
    GradientTo = "#3d0a0a",
    Icon = "🕷️",
    PosterUrl = PosterCatalog.Series("orumcek-adam"),
    BackdropUrl = PosterCatalog.Backdrop("orumcek-adam"),
    Movies = MoviesFrom("orumcek-adam",
    [
      (1, "Örümcek Adam: Eve Dönüş", 2017, 133, 7.4, "Peter Parker Avengers sonrası lise hayatına döner."),
      (2, "Örümcek Adam: Uzakta", 2019, 129, 7.4, "Avrupa tatili kabusa dönüşür."),
      (3, "Örümcek Adam: Eve Dönüş Yok", 2021, 148, 8.2, "Çoklu evren açılır; eski düşmanlar geri döner.")
    ]),
    Cast = Cast(
      ("Tom Holland", "Peter Parker", "Başrol"),
      ("Zendaya", "MJ", "Başrol"),
      ("Jacob Batalon", "Ned Leeds", "Yardımcı Rol"),
      ("Marisa Tomei", "May Parker", "Yardımcı Rol"),
      ("Jon Favreau", "Happy Hogan", "Yardımcı Rol"),
      ("Willem Dafoe", "Green Goblin", "Antagonist"))
  };

  private static MovieSeries BuildGodfather() => new()
  {
    Id = "baba",
    Title = "Baba",
    OriginalTitle = "The Godfather",
    Tagline = "Teklifini geri çevirme.",
    Description = "Francis Ford Coppola'nın mafya destanı, Corleone ailesinin Amerikan rüyası ve suç dünyasındaki yükselişini anlatır. Sinema tarihinin en büyük yapıtlarından biri.",
    Genre = "Dram / Suç",
    GenreKey = "drama",
    ReleaseYearStart = 1972,
    ReleaseYearEnd = 1990,
    Director = "Francis Ford Coppola",
    Studio = "Paramount Pictures",
    ImdbRating = 9.0,
    AccentColor = "#8b0000",
    GradientFrom = "#0a0a0a",
    GradientTo = "#1a0a0a",
    Icon = "🎩",
    PosterUrl = PosterCatalog.Series("baba"),
    BackdropUrl = PosterCatalog.Backdrop("baba"),
    Movies = MoviesFrom("baba",
    [
      (1, "Baba", 1972, 175, 9.2, "Vito Corleone'nin ailesi ve imparatorluğu."),
      (2, "Baba II", 1974, 202, 9.0, "Michael Corleone'nin güç mücadelesi ve genç Vito'nun hikâyesi."),
      (3, "Baba III", 1990, 162, 7.6, "Michael'ın pişmanlık ve kurtuluş arayışı.")
    ]),
    Cast = Cast(
      ("Marlon Brando", "Vito Corleone", "Başrol"),
      ("Al Pacino", "Michael Corleone", "Başrol"),
      ("James Caan", "Sonny Corleone", "Başrol"),
      ("Robert De Niro", "Young Vito", "Başrol"),
      ("Diane Keaton", "Kay Adams", "Yardımcı Rol"),
      ("John Cazale", "Fredo Corleone", "Yardımcı Rol"))
  };

  private static List<Movie> MoviesFrom(string seriesId, (int Order, string Title, int Year, int Duration, double Rating, string Synopsis)[] items) =>
    items.Select(m => new Movie
    {
      Order = m.Order,
      Title = m.Title,
      Year = m.Year,
      DurationMinutes = m.Duration,
      ImdbRating = m.Rating,
      Synopsis = m.Synopsis,
      PosterUrl = PosterCatalog.Movie(seriesId, m.Order)
    }).ToList();

  private static List<CastMember> Cast(params (string actor, string character, string role)[] members) =>
    members.Select(m => new CastMember
    {
      ActorName = m.actor,
      CharacterName = m.character,
      Role = m.role,
      PhotoUrl = ActorPhotos.Get(m.actor)
    }).ToList();
}

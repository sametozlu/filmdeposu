using FilmSerileri.Models;

namespace FilmSerileri.Services;

public class MovieService : IMovieService
{
  private static readonly List<MovieSeries> Series = BuildSeries();

  public IReadOnlyList<MovieSeries> GetAllSeries() => Series;

  public MovieSeries? GetSeriesById(string id) =>
    Series.FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

  public MovieSeries? GetFeaturedSeries() => Series.FirstOrDefault(s => s.Id == "harry-potter");

  private static List<MovieSeries> BuildSeries() =>
  [
    new MovieSeries
    {
      Id = "harry-potter",
      Title = "Harry Potter",
      OriginalTitle = "Harry Potter",
      Tagline = "Büyü seni bekliyor.",
      Description = "J.K. Rowling'in efsanevi kitap serisinden uyarlanan Harry Potter, genç bir büyücünün Hogwarts Cadılık ve Büyücülük Okulu'ndaki maceralarını anlatır. Dostluk, cesaret ve karanlığa karşı mücadele temalarıyla nesiller boyu izleyiciyi büyülemiştir.",
      Genre = "Fantastik / Macera",
      ReleaseYearStart = 2001,
      ReleaseYearEnd = 2011,
      Director = "Chris Columbus, Alfonso Cuarón, Mike Newell, David Yates",
      Studio = "Warner Bros.",
      ImdbRating = 7.6,
      AccentColor = "#d4a853",
      GradientFrom = "#1a0f2e",
      GradientTo = "#2d1b4e",
      Icon = "⚡",
      Movies =
      [
        new() { Order = 1, Title = "Harry Potter ve Felsefe Taşı", Year = 2001, DurationMinutes = 152, ImdbRating = 7.7, Synopsis = "Harry Potter, büyücü dünyasını keşfeder ve gizemli Felsefe Taşı'nı korur." },
        new() { Order = 2, Title = "Harry Potter ve Sırlar Odası", Year = 2002, DurationMinutes = 161, ImdbRating = 7.5, Synopsis = "Hogwarts'ta öğrenciler taşlaşmaya başlar; Sırlar Odası'nın sırrı çözülmelidir." },
        new() { Order = 3, Title = "Harry Potter ve Azkaban Tutsağı", Year = 2004, DurationMinutes = 142, ImdbRating = 7.9, Synopsis = "Kaçak mahkum Sirius Black, Harry'nin geçmişiyle bağlantılıdır." },
        new() { Order = 4, Title = "Harry Potter ve Ateş Kadehi", Year = 2005, DurationMinutes = 157, ImdbRating = 7.7, Synopsis = "Üçbüyü Turnuvası ve Voldemort'un geri dönüşü." },
        new() { Order = 5, Title = "Harry Potter ve Zümrüdüanka Yoldaşlığı", Year = 2007, DurationMinutes = 138, ImdbRating = 7.5, Synopsis = "Harry, Voldemort'a karşı direniş örgütü kurar." },
        new() { Order = 6, Title = "Harry Potter ve Melez Prens", Year = 2009, DurationMinutes = 153, ImdbRating = 7.6, Synopsis = "Voldemort'un geçmişi ve Horcrux'ların sırrı ortaya çıkar." },
        new() { Order = 7, Title = "Harry Potter ve Ölüm Yadigârları: Bölüm 1", Year = 2010, DurationMinutes = 146, ImdbRating = 7.7, Synopsis = "Harry, Ron ve Hermione Horcrux avına çıkar." },
        new() { Order = 8, Title = "Harry Potter ve Ölüm Yadigârları: Bölüm 2", Year = 2011, DurationMinutes = 130, ImdbRating = 8.1, Synopsis = "Hogwarts'ta son savaş: Harry ve Voldemort yüzleşir." }
      ],
      Cast =
      [
        new() { ActorName = "Daniel Radcliffe", CharacterName = "Harry Potter", Role = "Başrol" },
        new() { ActorName = "Emma Watson", CharacterName = "Hermione Granger", Role = "Başrol" },
        new() { ActorName = "Rupert Grint", CharacterName = "Ron Weasley", Role = "Başrol" },
        new() { ActorName = "Alan Rickman", CharacterName = "Severus Snape", Role = "Yardımcı Rol" },
        new() { ActorName = "Ralph Fiennes", CharacterName = "Lord Voldemort", Role = "Antagonist" },
        new() { ActorName = "Maggie Smith", CharacterName = "Minerva McGonagall", Role = "Yardımcı Rol" }
      ]
    },
    new MovieSeries
    {
      Id = "yuzuklerin-efendisi",
      Title = "Yüzüklerin Efendisi",
      OriginalTitle = "The Lord of the Rings",
      Tagline = "Tek Yüzük hepsini yönetecek.",
      Description = "J.R.R. Tolkien'in Orta Dünya destanı, hobbit Frodo Baggins'in Karanlık Lord Sauron'u durdurmak için Tek Yüzük'ü yok etme yolculuğunu anlatır. Epik savaşlar, derin karakterler ve görsel şölen ile sinema tarihinin en büyük üçlemelerinden biridir.",
      Genre = "Fantastik / Epik Macera",
      ReleaseYearStart = 2001,
      ReleaseYearEnd = 2003,
      Director = "Peter Jackson",
      Studio = "New Line Cinema",
      ImdbRating = 8.8,
      AccentColor = "#c9a227",
      GradientFrom = "#0d1f0d",
      GradientTo = "#1a3a1a",
      Icon = "💍",
      Movies =
      [
        new() { Order = 1, Title = "Yüzüklerin Efendisi: Yüzük Kardeşliği", Year = 2001, DurationMinutes = 178, ImdbRating = 8.8, Synopsis = "Frodo ve Yüzük Kardeşliği, Yüzük'ü Mordor'a götürmek için yola çıkar." },
        new() { Order = 2, Title = "Yüzüklerin Efendisi: İki Kule", Year = 2002, DurationMinutes = 179, ImdbRating = 8.8, Synopsis = "Kardeşlik dağılır; savaşlar ve ihanetler Orta Dünya'yı sarar." },
        new() { Order = 3, Title = "Yüzüklerin Efendisi: Kralın Dönüşü", Year = 2003, DurationMinutes = 201, ImdbRating = 9.0, Synopsis = "Son savaş: Gondor'un savunması ve Yüzük'ün yok edilişi." }
      ],
      Cast =
      [
        new() { ActorName = "Elijah Wood", CharacterName = "Frodo Baggins", Role = "Başrol" },
        new() { ActorName = "Viggo Mortensen", CharacterName = "Aragorn", Role = "Başrol" },
        new() { ActorName = "Ian McKellen", CharacterName = "Gandalf", Role = "Başrol" },
        new() { ActorName = "Sean Astin", CharacterName = "Samwise Gamgee", Role = "Başrol" },
        new() { ActorName = "Orlando Bloom", CharacterName = "Legolas", Role = "Yardımcı Rol" },
        new() { ActorName = "Andy Serkis", CharacterName = "Gollum", Role = "Yardımcı Rol" }
      ]
    },
    new MovieSeries
    {
      Id = "alacakaranlik",
      Title = "Alacakaranlık",
      OriginalTitle = "Twilight",
      Tagline = "Aşk ölümsüzdür.",
      Description = "Stephenie Meyer'in romantik fantastik serisi, insan Bella Swan ile vampir Edward Cullen arasındaki yasak aşkı konu alır. Washington'un yağmurlu Forks kasabasında geçen hikâye, gençlik ve aşk temalarını doğaüstü öğelerle birleştirir.",
      Genre = "Romantik / Fantastik",
      ReleaseYearStart = 2008,
      ReleaseYearEnd = 2012,
      Director = "Catherine Hardwicke, Chris Weitz, David Slade, Bill Condon",
      Studio = "Summit Entertainment",
      ImdbRating = 5.3,
      AccentColor = "#8b0000",
      GradientFrom = "#1a0a0a",
      GradientTo = "#3d1515",
      Icon = "🌙",
      Movies =
      [
        new() { Order = 1, Title = "Alacakaranlık", Year = 2008, DurationMinutes = 122, ImdbRating = 5.3, Synopsis = "Bella, gizemli Edward ile tanışır ve onun bir vampir olduğunu öğrenir." },
        new() { Order = 2, Title = "Alacakaranlık Efsanesi: Yeni Ay", Year = 2009, DurationMinutes = 130, ImdbRating = 4.8, Synopsis = "Edward ayrılır; Bella Jacob ile yakınlaşır ve kurt adamları keşfeder." },
        new() { Order = 3, Title = "Alacakaranlık Efsanesi: Tutulma", Year = 2010, DurationMinutes = 124, ImdbRating = 5.0, Synopsis = "Victoria'nın intikamı ve Edward ile Bella'nın yeniden bir araya gelişi." },
        new() { Order = 4, Title = "Alacakaranlık Efsanesi: Şafak Vakti – Bölüm 1", Year = 2011, DurationMinutes = 117, ImdbRating = 5.0, Synopsis = "Bella ve Edward evlenir; hamilelik beklenmedik tehlikeler getirir." },
        new() { Order = 5, Title = "Alacakaranlık Efsanesi: Şafak Vakti – Bölüm 2", Year = 2012, DurationMinutes = 115, ImdbRating = 5.5, Synopsis = "Volturi ile son yüzleşme ve Bella'nın vampir olma yolculuğu tamamlanır." }
      ],
      Cast =
      [
        new() { ActorName = "Kristen Stewart", CharacterName = "Bella Swan", Role = "Başrol" },
        new() { ActorName = "Robert Pattinson", CharacterName = "Edward Cullen", Role = "Başrol" },
        new() { ActorName = "Taylor Lautner", CharacterName = "Jacob Black", Role = "Başrol" },
        new() { ActorName = "Ashley Greene", CharacterName = "Alice Cullen", Role = "Yardımcı Rol" },
        new() { ActorName = "Peter Facinelli", CharacterName = "Carlisle Cullen", Role = "Yardımcı Rol" },
        new() { ActorName = "Nikki Reed", CharacterName = "Rosalie Hale", Role = "Yardımcı Rol" }
      ]
    },
    new MovieSeries
    {
      Id = "labirent",
      Title = "Labirent",
      OriginalTitle = "The Maze Runner",
      Tagline = "Kaçış tek seçenek.",
      Description = "James Dashner'ın distopik gençlik romanından uyarlanan seri, hafızasını kaybetmiş gençlerin dev bir labirentin ortasındaki Glade adlı toplulukta hayatta kalma mücadelesini anlatır. Gizem, aksiyon ve sürprizlerle dolu bir macera.",
      Genre = "Distopya / Bilim Kurgu / Aksiyon",
      ReleaseYearStart = 2014,
      ReleaseYearEnd = 2018,
      Director = "Wes Ball",
      Studio = "20th Century Fox",
      ImdbRating = 6.8,
      AccentColor = "#2ecc71",
      GradientFrom = "#0a1a14",
      GradientTo = "#143d2e",
      Icon = "🧩",
      Movies =
      [
        new() { Order = 1, Title = "Labirent: Ölümcül Kaçış", Year = 2014, DurationMinutes = 113, ImdbRating = 6.8, Synopsis = "Thomas uyanır ve kendini dev labirentin ortasında bulur." },
        new() { Order = 2, Title = "Labirent: Alev Deneyleri", Year = 2015, DurationMinutes = 132, ImdbRating = 6.3, Synopsis = "Hayatta kalanlar çorak topraklarda WCKD'ye karşı savaşır." },
        new() { Order = 3, Title = "Labirent: Ölümcül İlaç", Year = 2018, DurationMinutes = 143, ImdbRating = 5.4, Synopsis = "Son şans: antidotu bulmak ve WCKD'yi durdurmak." }
      ],
      Cast =
      [
        new() { ActorName = "Dylan O'Brien", CharacterName = "Thomas", Role = "Başrol" },
        new() { ActorName = "Kaya Scodelario", CharacterName = "Teresa Agnes", Role = "Başrol" },
        new() { ActorName = "Thomas Brodie-Sangster", CharacterName = "Newt", Role = "Yardımcı Rol" },
        new() { ActorName = "Ki Hong Lee", CharacterName = "Minho", Role = "Yardımcı Rol" },
        new() { ActorName = "Will Poulter", CharacterName = "Gally", Role = "Yardımcı Rol" },
        new() { ActorName = "Patricia Clarkson", CharacterName = "Ava Paige", Role = "Antagonist" }
      ]
    },
    new MovieSeries
    {
      Id = "aclik-oyunlari",
      Title = "Açlık Oyunları",
      OriginalTitle = "The Hunger Games",
      Tagline = "Hayatta kalmak için oyna.",
      Description = "Suzanne Collins'in distopik romanından uyarlanan seri, totaliter Panem ülkesinde gençlerin televizyonda yayınlanan ölümcül bir turnuvada savaşmasını konu alır. Katniss Everdeen'in cesareti ve direnişi, umut ve adalet temalarını öne çıkarır.",
      Genre = "Distopya / Bilim Kurgu / Aksiyon",
      ReleaseYearStart = 2012,
      ReleaseYearEnd = 2015,
      Director = "Gary Ross, Francis Lawrence",
      Studio = "Lionsgate",
      ImdbRating = 7.2,
      AccentColor = "#e74c3c",
      GradientFrom = "#1a0f0a",
      GradientTo = "#3d1f14",
      Icon = "🏹",
      Movies =
      [
        new() { Order = 1, Title = "Açlık Oyunları", Year = 2012, DurationMinutes = 142, ImdbRating = 7.2, Synopsis = "Katniss, kız kardeşi yerine Açlık Oyunları'na katılır." },
        new() { Order = 2, Title = "Açlık Oyunları: Ateşi Yakalamak", Year = 2013, DurationMinutes = 146, ImdbRating = 7.5, Synopsis = "Zafer turu ve isyanın kıvılcımı." },
        new() { Order = 3, Title = "Açlık Oyunları: Alaycı Kuş – Bölüm 1", Year = 2014, DurationMinutes = 123, ImdbRating = 6.7, Synopsis = "Katniss Mockingjay sembolü olur; Capitol'a savaş ilan edilir." },
        new() { Order = 4, Title = "Açlık Oyunları: Alaycı Kuş – Bölüm 2", Year = 2015, DurationMinutes = 137, ImdbRating = 6.9, Synopsis = "Capitol'a son saldırı ve Snow'un düşüşü." }
      ],
      Cast =
      [
        new() { ActorName = "Jennifer Lawrence", CharacterName = "Katniss Everdeen", Role = "Başrol" },
        new() { ActorName = "Josh Hutcherson", CharacterName = "Peeta Mellark", Role = "Başrol" },
        new() { ActorName = "Liam Hemsworth", CharacterName = "Gale Hawthorne", Role = "Başrol" },
        new() { ActorName = "Woody Harrelson", CharacterName = "Haymitch Abernathy", Role = "Yardımcı Rol" },
        new() { ActorName = "Elizabeth Banks", CharacterName = "Effie Trinket", Role = "Yardımcı Rol" },
        new() { ActorName = "Donald Sutherland", CharacterName = "Başkan Snow", Role = "Antagonist" }
      ]
    }
  ];
}

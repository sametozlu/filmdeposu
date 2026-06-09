using FilmSerileri.Models;

namespace FilmSerileri.Services;

public static class SeriesLocalizer
{
  private static readonly Dictionary<string, RoleLabels> RoleMap = new()
  {
    ["Başrol"] = new("Lead", "Başrol"),
    ["Yardımcı Rol"] = new("Supporting", "Yardımcı Rol"),
    ["Antagonist"] = new("Antagonist", "Antagonist"),
  };

  public static MovieSeries Localize(MovieSeries source, string language)
  {
    if (!language.Equals("en", StringComparison.OrdinalIgnoreCase))
      return source;

    if (!English.TryGetValue(source.Id, out var en))
      return source;

    var clone = Clone(source);
    clone.Title = en.Title ?? clone.Title;
    clone.Tagline = en.Tagline ?? clone.Tagline;
    clone.Description = en.Description ?? clone.Description;
    clone.Genre = en.Genre ?? clone.Genre;

    for (var i = 0; i < clone.Movies.Count; i++)
    {
      if (en.Movies.TryGetValue(clone.Movies[i].Order, out var movieEn))
      {
        clone.Movies[i].Title = movieEn.Title ?? clone.Movies[i].Title;
        clone.Movies[i].Synopsis = movieEn.Synopsis ?? clone.Movies[i].Synopsis;
      }
    }

    foreach (var member in clone.Cast)
    {
      if (RoleMap.TryGetValue(member.Role, out var role))
        member.Role = role.En;
    }

    return clone;
  }

  public static List<MovieSeries> LocalizeAll(IEnumerable<MovieSeries> series, string language) =>
    series.Select(s => Localize(s, language)).ToList();

  private static MovieSeries Clone(MovieSeries s) => new()
  {
    Id = s.Id,
    Title = s.Title,
    OriginalTitle = s.OriginalTitle,
    Tagline = s.Tagline,
    Description = s.Description,
    Genre = s.Genre,
    GenreKey = s.GenreKey,
    ReleaseYearStart = s.ReleaseYearStart,
    ReleaseYearEnd = s.ReleaseYearEnd,
    Director = s.Director,
    Studio = s.Studio,
    ImdbRating = s.ImdbRating,
    AccentColor = s.AccentColor,
    GradientFrom = s.GradientFrom,
    GradientTo = s.GradientTo,
    Icon = s.Icon,
    PosterUrl = s.PosterUrl,
    BackdropUrl = s.BackdropUrl,
    Movies = s.Movies.Select(m => new Movie
    {
      Order = m.Order,
      Title = m.Title,
      Year = m.Year,
      DurationMinutes = m.DurationMinutes,
      Synopsis = m.Synopsis,
      ImdbRating = m.ImdbRating,
      PosterUrl = m.PosterUrl
    }).ToList(),
    Cast = s.Cast.Select(c => new CastMember
    {
      ActorName = c.ActorName,
      CharacterName = c.CharacterName,
      Role = c.Role,
      PhotoUrl = c.PhotoUrl
    }).ToList()
  };

  private record RoleLabels(string En, string Tr);

  private record SeriesEn(string? Title, string? Tagline, string? Description, string? Genre, Dictionary<int, MovieEn> Movies);
  private record MovieEn(string? Title, string? Synopsis);

  private static readonly Dictionary<string, SeriesEn> English = new()
  {
    ["harry-potter"] = new(
      "Harry Potter", "Magic awaits you.",
      "Adapted from J.K. Rowling's legendary books, Harry Potter follows a young wizard's adventures at Hogwarts School of Witchcraft and Wizardry. Themes of friendship, courage, and fighting darkness have enchanted generations.",
      "Fantasy / Adventure",
      new()
      {
        [1] = new("Harry Potter and the Philosopher's Stone", "Harry discovers the wizarding world and protects the mysterious Philosopher's Stone."),
        [2] = new("Harry Potter and the Chamber of Secrets", "Students begin petrifying at Hogwarts; the Chamber of Secrets must be uncovered."),
        [3] = new("Harry Potter and the Prisoner of Azkaban", "Fugitive Sirius Black is connected to Harry's past."),
        [4] = new("Harry Potter and the Goblet of Fire", "The Triwizard Tournament and Voldemort's return."),
        [5] = new("Harry Potter and the Order of the Phoenix", "Harry forms a resistance against Voldemort."),
        [6] = new("Harry Potter and the Half-Blood Prince", "Voldemort's past and the secret of Horcruxes are revealed."),
        [7] = new("Harry Potter and the Deathly Hallows: Part 1", "Harry, Ron, and Hermione hunt for Horcruxes."),
        [8] = new("Harry Potter and the Deathly Hallows: Part 2", "The final battle at Hogwarts: Harry faces Voldemort.")
      }),
    ["yuzuklerin-efendisi"] = new(
      "The Lord of the Rings", "One Ring to rule them all.",
      "J.R.R. Tolkien's Middle-earth saga follows hobbit Frodo Baggins on a quest to destroy the One Ring and stop Dark Lord Sauron. Epic battles, deep characters, and visual splendor make it one of cinema's greatest trilogies.",
      "Fantasy / Epic Adventure",
      new()
      {
        [1] = new("The Fellowship of the Ring", "Frodo and the Fellowship set out to take the Ring to Mordor."),
        [2] = new("The Two Towers", "The Fellowship splits; war and betrayal engulf Middle-earth."),
        [3] = new("The Return of the King", "The final battle: Gondor's defense and the Ring's destruction.")
      }),
    ["alacakaranlik"] = new(
      "Twilight", "Love is immortal.",
      "Stephenie Meyer's romantic fantasy follows the forbidden love between human Bella Swan and vampire Edward Cullen. Set in rainy Forks, Washington, it blends youth and romance with supernatural elements.",
      "Romance / Fantasy",
      new()
      {
        [1] = new("Twilight", "Bella meets mysterious Edward and learns he is a vampire."),
        [2] = new("New Moon", "Edward leaves; Bella grows close to Jacob and discovers werewolves."),
        [3] = new("Eclipse", "Victoria's revenge and Bella and Edward's reunion."),
        [4] = new("Breaking Dawn – Part 1", "Bella and Edward marry; pregnancy brings unexpected dangers."),
        [5] = new("Breaking Dawn – Part 2", "Final confrontation with the Volturi; Bella becomes a vampire.")
      }),
    ["labirent"] = new(
      "The Maze Runner", "Escape is the only option.",
      "Based on James Dashner's dystopian novel, amnesiac teens fight to survive in the Glade, a community trapped inside a giant maze. A mystery-filled adventure packed with action and surprises.",
      "Dystopia / Sci-Fi / Action",
      new()
      {
        [1] = new("The Maze Runner", "Thomas wakes up in the center of a massive maze."),
        [2] = new("Maze Runner: The Scorch Trials", "Survivors battle WCKD across a wasteland."),
        [3] = new("Maze Runner: The Death Cure", "Last chance: find the antidote and stop WCKD.")
      }),
    ["aclik-oyunlari"] = new(
      "The Hunger Games", "Play to survive.",
      "Suzanne Collins' dystopian saga follows teens forced to fight in a televised death match in totalitarian Panem. Katniss Everdeen's courage and rebellion highlight themes of hope and justice.",
      "Dystopia / Sci-Fi / Action",
      new()
      {
        [1] = new("The Hunger Games", "Katniss volunteers in place of her sister."),
        [2] = new("Catching Fire", "The victory tour sparks the flames of rebellion."),
        [3] = new("Mockingjay – Part 1", "Katniss becomes the Mockingjay; war is declared on the Capitol."),
        [4] = new("Mockingjay – Part 2", "Final assault on the Capitol and Snow's downfall.")
      }),
    ["yildiz-savaslari"] = new(
      "Star Wars", "A galaxy far, far away...",
      "George Lucas's space opera spans generations of Jedi, Sith, and rebels. Lightsaber duels, iconic heroes, and an epic battle between good and evil defined modern blockbuster cinema.",
      "Sci-Fi / Space Opera / Adventure",
      new()
      {
        [1] = new("Episode I: The Phantom Menace", "Young Anakin Skywalker and the invasion of Naboo."),
        [2] = new("Episode II: Attack of the Clones", "The Clone Wars begin; Anakin and Padmé fall in love."),
        [3] = new("Episode III: Revenge of the Sith", "Anakin falls to the dark side; the Empire rises."),
        [4] = new("Episode IV: A New Hope", "Luke Skywalker joins the Rebellion against the Empire."),
        [5] = new("Episode V: The Empire Strikes Back", "Luke trains with Yoda; Vader's shocking revelation."),
        [6] = new("Episode VI: Return of the Jedi", "The Rebellion's final strike against the Empire."),
        [7] = new("Episode VII: The Force Awakens", "A new generation confronts the rising First Order."),
        [8] = new("Episode VIII: The Last Jedi", "Rey seeks Luke; the Resistance fights for survival."),
        [9] = new("Episode IX: The Rise of Skywalker", "The final battle against Emperor Palpatine.")
      }),
    ["karayip-korsanlari"] = new(
      "Pirates of the Caribbean", "Wherever we want to go, we'll go.",
      "Disney's swashbuckling adventure follows the eccentric Captain Jack Sparrow and his crew across cursed treasure, sea monsters, and supernatural foes on the high seas.",
      "Adventure / Fantasy / Action",
      new()
      {
        [1] = new("The Curse of the Black Pearl", "Jack Sparrow and Will Turner hunt cursed Aztec gold."),
        [2] = new("Dead Man's Chest", "Jack owes his soul to Davy Jones."),
        [3] = new("At World's End", "Pirates unite against the East India Company."),
        [4] = new("On Stranger Tides", "The search for the Fountain of Youth."),
        [5] = new("Dead Men Tell No Tales", "Jack faces the ghostly Captain Salazar.")
      }),
    ["jurassic-park"] = new(
      "Jurassic Park", "Life finds a way.",
      "Steven Spielberg's landmark franchise brings dinosaurs back to life through cloning. Science gone wrong, thrilling chases, and the awe of prehistoric creatures captivate audiences worldwide.",
      "Sci-Fi / Adventure / Thriller",
      new()
      {
        [1] = new("Jurassic Park", "Dinosaurs roam a theme park until systems fail."),
        [2] = new("The Lost World", "A second island of dinosaurs is discovered."),
        [3] = new("Jurassic Park III", "A rescue mission on Isla Sorna goes wrong."),
        [4] = new("Jurassic World", "A new park opens with a genetically modified hybrid."),
        [5] = new("Fallen Kingdom", "Dinosaurs face extinction after the island volcano erupts."),
        [6] = new("Dominion", "Dinosaurs now live among humans worldwide.")
      }),
    ["dune"] = new(
      "Dune", "He who controls the spice controls the universe.",
      "Frank Herbert's sci-fi epic follows Paul Atreides on the desert planet Arrakis, home to the most valuable substance in the universe. Politics, prophecy, and sandworms shape a grand saga.",
      "Sci-Fi / Epic / Adventure",
      new()
      {
        [1] = new("Dune: Part One", "Paul Atreides arrives on Arrakis; House Atreides is betrayed."),
        [2] = new("Dune: Part Two", "Paul unites the Fremen and challenges the Emperor.")
      }),
    ["matrix"] = new(
      "The Matrix", "Welcome to the real world.",
      "The Wachowskis' cult sci-fi saga depicts humanity trapped in a simulated reality by machines. Neo's awakening blends action and philosophy.",
      "Sci-Fi / Action",
      new()
      {
        [1] = new("The Matrix", "Neo learns the terrifying secret of the real world."),
        [2] = new("The Matrix Reloaded", "Zion is threatened; Neo discovers his powers."),
        [3] = new("The Matrix Revolutions", "Final war between humanity and machines."),
        [4] = new("The Matrix Resurrections", "Neo returns to the Matrix; old and new realities collide.")
      }),
    ["kara-sovalye"] = new(
      "The Dark Knight", "Darkness rises.",
      "Christopher Nolan's Batman trilogy follows Bruce Wayne's battle to save Gotham with a grounded, epic tone. Deep themes of heroism, terror, and justice.",
      "Action / Drama / Superhero",
      new()
      {
        [1] = new("Batman Begins", "Bruce Wayne becomes Batman and protects Gotham."),
        [2] = new("The Dark Knight", "The Joker plunges Gotham into chaos."),
        [3] = new("The Dark Knight Rises", "Bane besieges Gotham; Batman returns.")
      }),
    ["hizli-ve-ofkeli"] = new(
      "Fast & Furious", "Family comes first.",
      "From street racing to global espionage, Dominic Toretto and his family face loyalty, speed, and adrenaline-fueled adventures.",
      "Action / Adventure",
      new()
      {
        [1] = new("The Fast and the Furious", "An undercover cop infiltrates a street racing crew."),
        [2] = new("2 Fast 2 Furious", "Brian and Roman take on a mission in Miami."),
        [3] = new("Tokyo Drift", "Drift racing and the yakuza in Tokyo."),
        [4] = new("Fast & Furious", "Dom and Brian reunite."),
        [5] = new("Fast Five", "A big heist in Rio cements the family."),
        [6] = new("Fast & Furious 6", "A new enemy faces the crew."),
        [7] = new("Furious 7", "Deckard Shaw seeks revenge.")
      }),
    ["orumcek-adam"] = new(
      "Spider-Man", "With great power comes great responsibility.",
      "In the MCU, young Peter Parker becomes Spider-Man under Tony Stark's mentorship and ventures into the multiverse.",
      "Action / Superhero / Adventure",
      new()
      {
        [1] = new("Spider-Man: Homecoming", "Peter returns to high school after the Avengers."),
        [2] = new("Spider-Man: Far From Home", "A European vacation turns into a nightmare."),
        [3] = new("Spider-Man: No Way Home", "The multiverse opens; old foes return.")
      }),
    ["baba"] = new(
      "The Godfather", "An offer you can't refuse.",
      "Francis Ford Coppola's mafia saga follows the Corleone family's rise in the American dream and the criminal underworld. One of cinema's greatest works.",
      "Drama / Crime",
      new()
      {
        [1] = new("The Godfather", "Vito Corleone's family and empire."),
        [2] = new("The Godfather Part II", "Michael's power struggle and young Vito's story."),
        [3] = new("The Godfather Part III", "Michael's search for redemption.")
      })
  };
}

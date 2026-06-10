using System.Text.Json.Serialization;

namespace FilmSerileri.Services.Tmdb;

public class TmdbMovieDetails
{
  [JsonPropertyName("id")] public int Id { get; set; }
  [JsonPropertyName("title")] public string? Title { get; set; }
  [JsonPropertyName("overview")] public string? Overview { get; set; }
  [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
  [JsonPropertyName("runtime")] public int? Runtime { get; set; }
  [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }
  [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }
}

public class TmdbPersonDetails
{
  [JsonPropertyName("id")] public int Id { get; set; }
  [JsonPropertyName("name")] public string? Name { get; set; }
  [JsonPropertyName("biography")] public string? Biography { get; set; }
  [JsonPropertyName("place_of_birth")] public string? PlaceOfBirth { get; set; }
  [JsonPropertyName("profile_path")] public string? ProfilePath { get; set; }
}

public class TmdbVideosResponse
{
  [JsonPropertyName("results")] public List<TmdbVideo> Results { get; set; } = new();
}

public class TmdbVideo
{
  [JsonPropertyName("key")] public string? Key { get; set; }
  [JsonPropertyName("site")] public string? Site { get; set; }
  [JsonPropertyName("type")] public string? Type { get; set; }
  [JsonPropertyName("official")] public bool Official { get; set; }
}

public class TmdbCollectionResponse
{
  [JsonPropertyName("parts")] public List<TmdbCollectionPart> Parts { get; set; } = new();
}

public class TmdbCollectionPart
{
  [JsonPropertyName("id")] public int Id { get; set; }
  [JsonPropertyName("title")] public string? Title { get; set; }
  [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
}

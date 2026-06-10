namespace FilmSerileri.Options;

public class OmdbOptions
{
  public const string SectionName = "Omdb";
  public string ApiKey { get; set; } = string.Empty;
  public string BaseUrl { get; set; } = "https://www.omdbapi.com";
  public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}

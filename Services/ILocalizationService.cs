namespace FilmSerileri.Services;

public interface ILocalizationService
{
  string T(string key, string? language = null);
  string CurrentLanguage { get; }
}

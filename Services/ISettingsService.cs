using FilmSerileri.Models;

namespace FilmSerileri.Services;

public interface ISettingsService
{
    AppSettings GetSettings();
    void SaveSettings(AppSettings settings);
}

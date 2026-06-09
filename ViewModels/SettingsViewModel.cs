using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class SettingsViewModel
{
    public AppSettings Settings { get; set; } = new();
    public List<MovieSeries> AllSeries { get; set; } = new();
}

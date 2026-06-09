using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class SeriesDetailViewModel
{
    public MovieSeries Series { get; set; } = new();
    public AppSettings Settings { get; set; } = new();
    public MovieSeries? NextSeries { get; set; }
    public MovieSeries? PrevSeries { get; set; }
}

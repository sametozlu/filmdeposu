using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class HomeViewModel
{
    public List<MovieSeries> Series { get; set; } = new();
    public MovieSeries? FeaturedSeries { get; set; }
    public AppSettings Settings { get; set; } = new();
}

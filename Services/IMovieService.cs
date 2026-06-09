using FilmSerileri.Models;

namespace FilmSerileri.Services;

public interface IMovieService
{
    IReadOnlyList<MovieSeries> GetAllSeries();
    MovieSeries? GetSeriesById(string id);
    MovieSeries? GetFeaturedSeries();
}

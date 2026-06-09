using FilmSerileri.Models;

namespace FilmSerileri.Services;

public interface IMovieService
{
    IReadOnlyList<MovieSeries> GetAllSeries(string language = "tr");
    MovieSeries? GetSeriesById(string id, string language = "tr");
    MovieSeries? GetFeaturedSeries(string language = "tr");
    IReadOnlyList<(string Key, string Label)> GetGenres(string language = "tr");
    IReadOnlyList<MovieSeries> SearchSeries(string? query, string? genre, double? minRating, string sortBy, string language = "tr");
}

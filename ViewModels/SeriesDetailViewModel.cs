using FilmSerileri.Entities;
using FilmSerileri.Models;

namespace FilmSerileri.ViewModels;

public class SeriesDetailViewModel
{
  public MovieSeries Series { get; set; } = new();
  public AppSettings Settings { get; set; } = new();
  public MovieSeries? NextSeries { get; set; }
  public MovieSeries? PrevSeries { get; set; }
  public IReadOnlyList<MovieSeries> SimilarSeries { get; set; } = Array.Empty<MovieSeries>();
  public bool IsInWatchlist { get; set; }
  public string? UserNote { get; set; }
  public bool IsAuthenticated { get; set; }
  public IReadOnlyList<Review> Reviews { get; set; } = Array.Empty<Review>();
  public Review? UserReview { get; set; }
  public double? AverageUserRating { get; set; }
}

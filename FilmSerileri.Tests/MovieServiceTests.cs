using FilmSerileri.Services;
using Xunit;
using FilmSerileri.Services.Omdb;
using FilmSerileri.Services.Tmdb;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace FilmSerileri.Tests;

public class MovieServiceTests
{
  private readonly IMovieService _service;

  public MovieServiceTests()
  {
    var tmdb = new Mock<ITmdbService>();
    tmdb.Setup(t => t.IsAvailable).Returns(false);
    var omdb = new Mock<IOmdbService>();
    omdb.Setup(o => o.IsAvailable).Returns(false);

    var enrichment = new MovieEnrichmentService(
      tmdb.Object,
      omdb.Object,
      Microsoft.Extensions.Options.Options.Create(new FilmSerileri.Options.TmdbOptions()),
      new MemoryCache(new MemoryCacheOptions()),
      NullLogger<MovieEnrichmentService>.Instance);

    _service = new MovieService(enrichment, new SeriesCatalogService());
  }

  [Fact]
  public void GetAllSeries_Returns17Franchises()
  {
    var all = _service.GetAllSeries();
    Assert.Equal(17, all.Count);
  }

  [Fact]
  public void SearchSeries_FiltersByQuery()
  {
    var results = _service.SearchSeries("harry", null, null, "rating");
    Assert.All(results, s => Assert.Contains("harry", s.Title.ToLowerInvariant() + s.Id));
  }

  [Fact]
  public void GetSimilarSeries_ExcludesSelf()
  {
    var similar = _service.GetSimilarSeries("harry-potter");
    Assert.DoesNotContain(similar, s => s.Id == "harry-potter");
    Assert.NotEmpty(similar);
  }

  [Fact]
  public void CompareSeries_ReturnsBoth()
  {
    var cmp = _service.CompareSeries("harry-potter", "matrix");
    Assert.NotNull(cmp);
    Assert.Equal("harry-potter", cmp!.SeriesA.Id);
    Assert.Equal("matrix", cmp.SeriesB.Id);
  }

  [Fact]
  public void SearchSeriesPaged_ReturnsCorrectPageSize()
  {
    var page = _service.SearchSeriesPaged(null, null, null, "rating", 1, 6);
    Assert.Equal(6, page.Items.Count);
    Assert.True(page.TotalCount >= 14);
  }
}

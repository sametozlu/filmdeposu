using FilmSerileri.Models;
using Xunit;
using FilmSerileri.Services;
using Moq;

namespace FilmSerileri.Tests;

public class LocalizationServiceTests
{
  [Fact]
  public void T_ReturnsEnglishWhenRequested()
  {
    var settings = new Mock<ISettingsService>();
    settings.Setup(s => s.GetSettings()).Returns(new AppSettings { Language = "en" });
    var loc = new LocalizationService(settings.Object);
    Assert.Equal("Home", loc.T("nav_home", "en"));
    Assert.Equal("Ana Sayfa", loc.T("nav_home", "tr"));
  }
}

using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace FilmSerileri.E2E;

/// <summary>
/// Çalışan uygulamaya karşı tarayıcıyla uçtan uca duman testleri.
/// Önkoşul: uygulama E2E_BASE_URL'de (varsayılan http://localhost:8080) çalışıyor olmalı.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SmokeTests : PageTest
{
  private static readonly string BaseUrl =
    Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:8080";

  [Test]
  public async Task HomePage_Loads_AndShowsSeriesCards()
  {
    await Page.GotoAsync(BaseUrl);
    await Expect(Page).ToHaveTitleAsync(new Regex("Sinema|Cinema"));
    await Expect(Page.Locator(".marquee-item").First).ToBeVisibleAsync();
  }

  [Test]
  public async Task SeriesDetail_ShowsTitleAndMovies()
  {
    await Page.GotoAsync($"{BaseUrl}/Series/Detail/harry-potter");
    await Expect(Page.Locator("h1")).ToContainTextAsync("Harry Potter");
    await Expect(Page.Locator(".movie-item").First).ToBeVisibleAsync();
  }

  [Test]
  public async Task CommandPalette_OpensAndFindsResults()
  {
    await Page.GotoAsync(BaseUrl);
    await Page.Keyboard.PressAsync("Control+k");
    await Expect(Page.Locator(".cp-input")).ToBeVisibleAsync();
    await Page.Locator(".cp-input").FillAsync("harry");
    await Expect(Page.Locator(".cp-item").First).ToBeVisibleAsync();
  }

  [Test]
  public async Task Quiz_StartsAndShowsQuestion()
  {
    await Page.GotoAsync($"{BaseUrl}/Quiz");
    await Page.Locator("#quizStartBtn").ClickAsync();
    await Expect(Page.Locator("#quizQuestion")).ToBeVisibleAsync();
    await Expect(Page.Locator(".quiz-option")).ToHaveCountAsync(4);
  }

  [Test]
  public async Task HealthEndpoint_ReturnsHealthy()
  {
    var response = await Page.APIRequest.GetAsync($"{BaseUrl}/health");
    Assert.That(response.Status, Is.EqualTo(200));
  }
}

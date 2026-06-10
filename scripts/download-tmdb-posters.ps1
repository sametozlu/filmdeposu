$ErrorActionPreference = "Continue"
$outDir = Join-Path $PSScriptRoot "..\wwwroot\images\posters"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function Get-TmdbPosterPath($movieId) {
  $url = "https://www.themoviedb.org/movie/$movieId"
  $html = (Invoke-WebRequest -Uri $url -UseBasicParsing -Headers @{"User-Agent" = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" }).Content
  if ($html -match 'https://media\.themoviedb\.org/t/p/w\d+/([a-zA-Z0-9]+\.jpg)') {
    return $matches[1]
  }
  return $null
}

function Save-TmdbPoster($path, $filename) {
  if (-not $path) { return }
  $url = "https://image.tmdb.org/t/p/w500/$path"
  $out = Join-Path $outDir $filename
  Invoke-WebRequest -Uri $url -OutFile $out -Headers @{"User-Agent" = "FilmSerileri/1.0" }
  Write-Host "OK $filename"
}

# TMDB film ID'leri
$series = [ordered]@{
  "harry-potter" = @(671, 672, 673, 674, 675, 767, 12444, 12445)
  "yuzuklerin-efendisi" = @(120, 121, 122)
  "alacakaranlik" = @(8966, 18239, 24021, 50619, 50620)
  "labirent" = @(198663, 294254, 336843)
  "aclik-oyunlari" = @(70160, 101299, 131631, 131634)
  "yildiz-savaslari" = @(1893, 1894, 1895, 11, 1891, 1892, 140607, 181808, 181812)
  "karayip-korsanlari" = @(22, 58, 285, 1865, 166426)
  "jurassic-park" = @(329, 330, 331, 135397, 351286, 507086)
  "dune" = @(438631, 693134)
  "matrix" = @(603, 604, 605, 624860)
  "kara-sovalye" = @(272, 155, 49026)
  "hizli-ve-ofkeli" = @(9799, 584, 9615, 13804, 51497, 82992, 168259)
  "orumcek-adam" = @(315635, 429617, 634649)
  "baba" = @(238, 240, 242)
  "john-wick" = @(245891, 324552, 458156, 603692)
  "gorevimiz-tehlike" = @(954, 955, 956, 56292, 177677, 353081, 575264)
  "yaratik" = @(348, 679, 8077, 8078)
}

foreach ($seriesId in $series.Keys) {
  $ids = $series[$seriesId]
  for ($i = 0; $i -lt $ids.Count; $i++) {
    try {
      $poster = Get-TmdbPosterPath $ids[$i]
      Save-TmdbPoster $poster "$seriesId-$($i+1).jpg"
    } catch { Write-Warning "FAIL $seriesId-$($i+1) id=$($ids[$i])" }
    Start-Sleep -Milliseconds 400
  }
  $first = Join-Path $outDir "$seriesId-1.jpg"
  if (Test-Path $first) { Copy-Item $first (Join-Path $outDir "$seriesId.jpg") -Force }
}

Write-Host "Done"

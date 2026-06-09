$posters = @(
  @{ Id="harry-potter"; Title="Harry Potter"; Icon="⚡"; From="#1a0f2e"; To="#2d1b4e"; Accent="#d4a853"; Movies=8 },
  @{ Id="yuzuklerin-efendisi"; Title="LOTR"; Icon="💍"; From="#0d1f0d"; To="#1a3a1a"; Accent="#c9a227"; Movies=3 },
  @{ Id="alacakaranlik"; Title="Twilight"; Icon="🌙"; From="#1a0a0a"; To="#3d1515"; Accent="#8b0000"; Movies=5 },
  @{ Id="labirent"; Title="Maze Runner"; Icon="🧩"; From="#0a1a14"; To="#143d2e"; Accent="#2ecc71"; Movies=3 },
  @{ Id="aclik-oyunlari"; Title="Hunger Games"; Icon="🏹"; From="#1a0f0a"; To="#3d1f14"; Accent="#e74c3c"; Movies=4 },
  @{ Id="yildiz-savaslari"; Title="Star Wars"; Icon="⭐"; From="#0a0a1a"; To="#1a1a3a"; Accent="#ffd700"; Movies=9 },
  @{ Id="karayip-korsanlari"; Title="Pirates"; Icon="🏴‍☠️"; From="#1a1008"; To="#3d2810"; Accent="#8B4513"; Movies=5 },
  @{ Id="jurassic-park"; Title="Jurassic Park"; Icon="🦖"; From="#0a1a0a"; To="#1a3d1a"; Accent="#228B22"; Movies=6 },
  @{ Id="dune"; Title="Dune"; Icon="🏜️"; From="#1a1408"; To="#3d3010"; Accent="#c2a366"; Movies=2 }
)

$outDir = Join-Path $PSScriptRoot "..\wwwroot\posters"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function New-PosterSvg($title, $icon, $from, $to, $accent, $subtitle) {
@"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 300 450" width="300" height="450">
  <defs>
    <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" style="stop-color:$from"/>
      <stop offset="100%" style="stop-color:$to"/>
    </linearGradient>
  </defs>
  <rect width="300" height="450" fill="url(#bg)"/>
  <rect x="0" y="380" width="300" height="70" fill="$accent" opacity="0.25"/>
  <text x="150" y="180" text-anchor="middle" font-size="72">$icon</text>
  <text x="150" y="300" text-anchor="middle" fill="#fff" font-family="Georgia,serif" font-size="22" font-weight="bold">$title</text>
  <text x="150" y="330" text-anchor="middle" fill="$accent" font-family="Arial,sans-serif" font-size="13">$subtitle</text>
  <rect x="20" y="20" width="260" height="410" fill="none" stroke="$accent" stroke-width="2" opacity="0.4" rx="4"/>
</svg>
"@
}

foreach ($p in $posters) {
  $seriesPath = Join-Path $outDir "$($p.Id).svg"
  New-PosterSvg $p.Title $p.Icon $p.From $p.To $p.Accent "FRANCHISE" | Set-Content $seriesPath -Encoding UTF8

  for ($i = 1; $i -le $p.Movies; $i++) {
    $moviePath = Join-Path $outDir "$($p.Id)-$i.svg"
    New-PosterSvg $p.Title $p.Icon $p.From $p.To $p.Accent "PART $i" | Set-Content $moviePath -Encoding UTF8
  }
}

Write-Host "Generated posters in $outDir"

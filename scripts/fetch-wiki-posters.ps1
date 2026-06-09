$ErrorActionPreference = "Continue"
$outDir = Join-Path $PSScriptRoot "..\wwwroot\images\posters"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function Get-WikiThumb($wikiPage) {
  $url = "https://en.wikipedia.org/api/rest_v1/page/summary/$wikiPage"
  $j = Invoke-RestMethod -Uri $url -Headers @{"User-Agent" = "FilmSerileri/1.0" }
  return $j.thumbnail.source
}

function Save-Image($url, $filename) {
  $path = Join-Path $outDir $filename
  Invoke-WebRequest -Uri $url -OutFile $path -Headers @{"User-Agent" = "FilmSerileri/1.0" }
  Write-Host "OK $filename"
}

# Wikipedia sayfa basliklari (underscore format)
$catalog = [ordered]@{
  "harry-potter" = @(
    "Harry_Potter_and_the_Philosopher%27s_Stone_(film)",
    "Harry_Potter_and_the_Chamber_of_Secrets_(film)",
    "Harry_Potter_and_the_Prisoner_of_Azkaban_(film)",
    "Harry_Potter_and_the_Goblet_of_Fire_(film)",
    "Harry_Potter_and_the_Order_of_the_Phoenix_(film)",
    "Harry_Potter_and_the_Half-Blood_Prince_(film)",
    "Harry_Potter_and_the_Deathly_Hallows_%E2%80%93_Part_1",
    "Harry_Potter_and_the_Deathly_Hallows_%E2%80%93_Part_2"
  )
  "yuzuklerin-efendisi" = @(
    "The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring",
    "The_Lord_of_the_Rings:_The_Two_Towers",
    "The_Lord_of_the_Rings:_The_Return_of_the_King"
  )
  "alacakaranlik" = @(
    "Twilight_(2008_film)", "The_Twilight_Saga:_New_Moon", "The_Twilight_Saga:_Eclipse",
    "The_Twilight_Saga:_Breaking_Dawn_%E2%80%93_Part_1", "The_Twilight_Saga:_Breaking_Dawn_%E2%80%93_Part_2"
  )
  "labirent" = @("The_Maze_Runner_(film)", "Maze_Runner:_The_Scorch_Trials", "Maze_Runner:_The_Death_Cure")
  "aclik-oyunlari" = @(
    "The_Hunger_Games_(film)", "The_Hunger_Games:_Catching_Fire",
    "The_Hunger_Games:_Mockingjay_%E2%80%93_Part_1", "The_Hunger_Games:_Mockingjay_%E2%80%93_Part_2"
  )
  "yildiz-savaslari" = @(
    "Star_Wars:_Episode_I_%E2%80%93_The_Phantom_Menace",
    "Star_Wars:_Episode_II_%E2%80%93_Attack_of_the_Clones",
    "Star_Wars:_Episode_III_%E2%80%93_Revenge_of_the_Sith",
    "Star_Wars_(film)", "The_Empire_Strikes_Back", "Return_of_the_Jedi",
    "Star_Wars:_The_Force_Awakens", "Star_Wars:_The_Last_Jedi", "Star_Wars:_The_Rise_of_Skywalker"
  )
  "karayip-korsanlari" = @(
    "Pirates_of_the_Caribbean:_The_Curse_of_the_Black_Pearl",
    "Pirates_of_the_Caribbean:_Dead_Man%27s_Chest",
    "Pirates_of_the_Caribbean:_At_World%27s_End",
    "Pirates_of_the_Caribbean:_On_Stranger_Tides",
    "Pirates_of_the_Caribbean:_Dead_Men_Tell_No_Tales"
  )
  "jurassic-park" = @(
    "Jurassic_Park_(film)", "The_Lost_World:_Jurassic_Park", "Jurassic_Park_III",
    "Jurassic_World", "Jurassic_World:_Fallen_Kingdom", "Jurassic_World_Dominion"
  )
  "dune" = @("Dune_(2021_film)", "Dune:_Part_Two")
  "matrix" = @("The_Matrix", "The_Matrix_Reloaded", "The_Matrix_Revolutions", "The_Matrix_Resurrections")
  "kara-sovalye" = @("Batman_Begins", "The_Dark_Knight", "The_Dark_Knight_Rises")
  "hizli-ve-ofkeli" = @(
    "The_Fast_and_the_Furious_(2001_film)", "2_Fast_2_Furious",
    "The_Fast_and_the_Furious:_Tokyo_Drift", "Fast_%26_Furious_(2009_film)",
    "Fast_Five", "Furious_7", "Furious_7"
  )
  "orumcek-adam" = @("Spider-Man:_Homecoming", "Spider-Man:_Far_From_Home", "Spider-Man:_No_Way_Home")
  "baba" = @("The_Godfather", "The_Godfather_Part_II", "The_Godfather_Part_III")
}

foreach ($id in $catalog.Keys) {
  $pages = $catalog[$id]
  $i = 1
  foreach ($page in $pages) {
    try {
      $thumb = Get-WikiThumb $page
      Save-Image $thumb "$id-$i.jpg"
    } catch { Write-Warning "FAIL $id-$i ($page)" }
    $i++
    Start-Sleep -Milliseconds 200
  }
  $first = Join-Path $outDir "$id-1.jpg"
  if (Test-Path $first) { Copy-Item $first (Join-Path $outDir "$id.jpg") -Force }
}

Write-Host "Done"

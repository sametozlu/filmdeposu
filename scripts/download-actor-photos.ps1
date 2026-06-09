$ErrorActionPreference = "Continue"
$outDir = Join-Path $PSScriptRoot "..\wwwroot\images\actors"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$ids = @{
  "daniel-radcliffe" = 10980
  "emma-watson" = 10990
  "rupert-grint" = 10989
  "alan-rickman" = 4566
  "ralph-fiennes" = 5469
  "maggie-smith" = 10978
  "elijah-wood" = 109
  "viggo-mortensen" = 8784
  "ian-mckellen" = 1327
  "sean-astin" = 608
  "orlando-bloom" = 114
  "andy-serkis" = 1709
  "kristen-stewart" = 37917
  "robert-pattinson" = 37125
  "taylor-lautner" = 84214
  "dylan-obrien" = 83968
  "kaya-scodelario" = 56731
  "thomas-brodie-sangster" = 221018
  "will-poulter" = 93491
  "patricia-clarkson" = 1276
  "jennifer-lawrence" = 72129
  "josh-hutcherson" = 996701
  "liam-hemsworth" = 96066
  "woody-harrelson" = 57795
  "elizabeth-banks" = 4587
  "donald-sutherland" = 2228
  "mark-hamill" = 2
  "harrison-ford" = 3
  "carrie-fisher" = 4
  "daisy-ridley" = 1315036
  "adam-driver" = 1023139
  "ewan-mcgregor" = 3061
  "johnny-depp" = 85
  "keira-knightley" = 116
  "geoffrey-rush" = 6573
  "bill-nighy" = 2440
  "javier-bardem" = 3810
  "sam-neill" = 4783
  "laura-dern" = 1231
  "jeff-goldblum" = 4785
  "chris-pratt" = 73457
  "bryce-dallas-howard" = 11664
  "timothee-chalamet" = 1190668
  "zendaya" = 505710
  "rebecca-ferguson" = 1373737
  "oscar-isaac" = 25072
  "josh-brolin" = 16828
  "keanu-reeves" = 6384
  "laurence-fishburne" = 2975
  "carrie-anne-moss" = 530
  "hugo-weaving" = 1331
  "christian-bale" = 3894
  "heath-ledger" = 1810
  "aaron-eckhart" = 6383
  "michael-caine" = 3895
  "gary-oldman" = 64
  "tom-hardy" = 2524
  "vin-diesel" = 12835
  "paul-walker" = 8167
  "michelle-rodriguez" = 17647
  "dwayne-johnson" = 18918
  "jason-statham" = 976
  "jordana-brewster" = 22160
  "tom-holland" = 1136406
  "jacob-batalon" = 1525043
  "marisa-tomei" = 1896
  "jon-favreau" = 15277
  "willem-dafoe" = 5293
  "marlon-brando" = 3084
  "al-pacino" = 1158
  "james-caan" = 3085
  "robert-de-niro" = 380
  "diane-keaton" = 3092
  "john-cazale" = 3095
  "ashley-greene" = 45827
  "peter-facinelli" = 56857
  "nikki-reed" = 59252
  "ki-hong-lee" = 1310760
  "jada-pinkett-smith" = 9575
  "lambert-wilson" = 2192
  "richard-attenborough" = 4786
}

function Get-ProfileImageUrl($personId) {
  $url = "https://www.themoviedb.org/person/$personId"
  $html = (Invoke-WebRequest -Uri $url -UseBasicParsing -Headers @{"User-Agent" = "Mozilla/5.0" }).Content
  if ($html -match 'property="og:image" content="([^"]+)"') {
    return $matches[1]
  }
  return $null
}

foreach ($slug in $ids.Keys) {
  $out = Join-Path $outDir "$slug.jpg"
  if ((Test-Path $out) -and (Get-Item $out).Length -gt 3000) {
    Write-Host "SKIP $slug"
    continue
  }
  try {
    $imgUrl = Get-ProfileImageUrl $ids[$slug]
    if ($imgUrl) {
      Invoke-WebRequest -Uri $imgUrl -OutFile $out -Headers @{"User-Agent" = "FilmSerileri/1.0" }
      Write-Host "OK $slug"
    } else { Write-Warning "No photo $slug" }
  } catch { Write-Warning "FAIL $slug : $_" }
  Start-Sleep -Milliseconds 400
}

Write-Host "Done"

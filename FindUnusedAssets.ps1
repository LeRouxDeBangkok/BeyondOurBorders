# --- CONFIGURATION ---
$projectPath = Get-Location
$reportFile = "Unused_Assets_Report.txt"

# Files to SCAN (Where references might be hiding)
# We look inside Scenes, Resources, C# Scripts, Godot Project settings
$sourceExtensions = @(".tscn", ".tres", ".cs", ".gd", ".godot", ".csproj", ".json")

# Files to CHECK (The assets we want to clean up)
# Add any other extensions you use (e.g., .ttf, .ogg)
$assetExtensions = @(".png", ".jpg", ".jpeg", ".wav", ".mp3", ".ogg", ".ttf", ".aseprite")

# --- EXECUTION ---
Write-Host "---------------------------------------------------" -ForegroundColor Cyan
Write-Host "STARTING ASSET SCAN IN: $projectPath" -ForegroundColor Cyan
Write-Host "This logic checks if an asset filename appears in any source file."
Write-Host "---------------------------------------------------"

# 1. Read all Source Code and Scene files into memory
Write-Host "Step 1/3: Reading all project source files..." -ForegroundColor Yellow
$allSourceText = ""
$sourceFiles = Get-ChildItem -Path $projectPath -Recurse -Include $sourceExtensions | Where-Object { $_.FullName -notmatch "\\.godot\\" -and $_.FullName -notmatch "\\.git\\" }

foreach ($file in $sourceFiles) {
    try {
        $content = [System.IO.File]::ReadAllText($file.FullName)
        $allSourceText += $content
    }
    catch {
        Write-Warning "Could not read $($file.Name)"
    }
}
Write-Host "-> Scanned $($sourceFiles.Count) source files." -ForegroundColor Green

# 2. Check Assets against the text
Write-Host "Step 2/3: Checking Assets..." -ForegroundColor Yellow
$assets = Get-ChildItem -Path "$projectPath\Assets" -Recurse | Where-Object { ! $_.PSIsContainer -and ($assetExtensions -contains $_.Extension) }
$unusedAssets = @()

foreach ($asset in $assets) {
    # We search for the filename (e.g. "player.png"). 
    # This is safer than full path, but might have rare false positives (which is good, we don't want to delete used stuff).
    $fileName = $asset.Name
    
    # Use Regex to check if the filename exists in the source text
    if ($allSourceText -notmatch [regex]::Escape($fileName)) {
        $unusedAssets += $asset.FullName
    }
}

# 3. Generate Report
Write-Host "Step 3/3: Generating Report..." -ForegroundColor Yellow
if ($unusedAssets.Count -gt 0) {
    $unusedAssets | Out-File $reportFile
    Write-Host "DONE! Found $($unusedAssets.Count) unused files." -ForegroundColor Red
    Write-Host "Check the file: $reportFile" -ForegroundColor White
} else {
    Write-Host "Amazing! No unused assets found." -ForegroundColor Green
}

Write-Host "---------------------------------------------------"
Write-Host "WARNING: Always double-check before deleting." -ForegroundColor Red
Write-Host "Assets loaded dynamically via code (e.g. 'Load(name + i)') might be flagged falsely."
Write-Host "---------------------------------------------------"
Pause
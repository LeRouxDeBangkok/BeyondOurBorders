# --- CONFIGURATION ---
$projectPath = Get-Location
$trashPath = "$projectPath\_TRASH_BIN"
$listFile = "$projectPath\FilesToDelete.txt"

# Create the trash bin
if (-not (Test-Path $trashPath)) {
    New-Item -ItemType Directory -Force -Path $trashPath | Out-Null
}

Write-Host "Reading file list..." -ForegroundColor Cyan
if (-not (Test-Path $listFile)) {
    Write-Error "Could not find FilesToDelete.txt! Please create it first."
    exit
}

$filesToMove = Get-Content $listFile

foreach ($filePath in $filesToMove) {
    # Clean up the path (remove whitespace)
    $cleanPath = $filePath.Trim()
    
    if (Test-Path $cleanPath) {
        # 1. Calculate the relative path to preserve structure
        # This turns "C:\Project\Assets\Sprite.png" into "Assets\Sprite.png"
        $relativePath = $cleanPath.Substring($projectPath.Path.Length + 1)
        
        # 2. Create the destination folder inside _TRASH_BIN
        $destFile = Join-Path $trashPath $relativePath
        $destFolder = Split-Path $destFile
        
        if (-not (Test-Path $destFolder)) {
            New-Item -ItemType Directory -Force -Path $destFolder | Out-Null
        }

        # 3. Move the file
        Move-Item -Path $cleanPath -Destination $destFile -Force
        Write-Host "Moved: $relativePath" -ForegroundColor Gray

        # 4. Move the associated .import file (Godot creates these)
        $importFile = "$cleanPath.import"
        if (Test-Path $importFile) {
            Move-Item -Path $importFile -Destination "$destFile.import" -Force
        }
    }
    else {
        Write-Warning "File not found (already moved?): $cleanPath"
    }
}

Write-Host "---------------------------------------------------"
Write-Host "DONE! Files moved to: $trashPath" -ForegroundColor Green
Write-Host "1. Open Godot and Run your game." -ForegroundColor Yellow
Write-Host "2. If assets are missing (purple squares), look in _TRASH_BIN and move them back." -ForegroundColor Yellow
Write-Host "3. If the game runs perfectly, DELETE the _TRASH_BIN folder." -ForegroundColor Red
Write-Host "---------------------------------------------------"
Pause
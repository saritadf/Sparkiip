# Unity Cache Cleaner for Sparkiip
# Cleans Unity compilation cache to force recompilation from source

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Unity Cache Cleaner for Sparkiip" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if Unity is running
$unityProcesses = Get-Process -Name "Unity" -ErrorAction SilentlyContinue
if ($unityProcesses) {
    Write-Host "WARNING: Unity is currently running!" -ForegroundColor Yellow
    Write-Host "Please close Unity before running this script." -ForegroundColor Yellow
    Write-Host ""
    $response = Read-Host "Continue anyway? (y/N)"
    if ($response -ne "y" -and $response -ne "Y") {
        Write-Host "Aborted by user." -ForegroundColor Red
        exit 1
    }
}

Write-Host "Starting cache cleanup..." -ForegroundColor Green
Write-Host ""

# Function to safely remove directory
function Remove-DirectorySafely {
    param (
        [string]$Path,
        [string]$Description
    )
    
    if (Test-Path $Path) {
        Write-Host "Removing $Description..." -ForegroundColor Yellow
        try {
            Remove-Item -Path $Path -Recurse -Force -ErrorAction Stop
            Write-Host "  [OK] Removed: $Path" -ForegroundColor Green
        }
        catch {
            Write-Host "  [ERROR] Failed to remove: $Path" -ForegroundColor Red
            Write-Host "    Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  [SKIP] Not found: $Path" -ForegroundColor Gray
    }
}

# Clean Library/ScriptAssemblies (compiled scripts)
Remove-DirectorySafely -Path "Library\ScriptAssemblies" -Description "Script Assemblies"

# Clean Temp/Bin (temporary build files)
Remove-DirectorySafely -Path "Temp\Bin" -Description "Temp Build Files"

# Clean Temp/ProcessJobs (build job cache)
Remove-DirectorySafely -Path "Temp\ProcessJobs" -Description "Process Jobs Cache"

# Clean obj/Debug (C# project debug files)
Remove-DirectorySafely -Path "obj\Debug" -Description "Debug Build Files"

# Optional: Regenerate .meta files for Scripts
Write-Host ""
Write-Host "Regenerating .meta files for Scripts folder..." -ForegroundColor Yellow
$metaFiles = Get-ChildItem -Path "Assets\Scripts" -Filter "*.meta" -ErrorAction SilentlyContinue
if ($metaFiles) {
    $metaFiles | Remove-Item -Force -ErrorAction SilentlyContinue
    Write-Host "  [OK] Removed $($metaFiles.Count) .meta files" -ForegroundColor Green
    Write-Host "  [INFO] Unity will regenerate them on next startup" -ForegroundColor Cyan
}
else {
    Write-Host "  [SKIP] No .meta files found" -ForegroundColor Gray
}

Write-Host ""
Write-Host "================================" -ForegroundColor Green
Write-Host "Cache cleanup completed!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Open Unity" -ForegroundColor White
Write-Host "  2. Wait for recompilation to complete" -ForegroundColor White
Write-Host "  3. Check Console for any remaining errors" -ForegroundColor White
Write-Host ""

# Simple installer creator for Auto_Updater
# This script creates a self-extracting installer

param(
    [string]$Version = "1.0.0"
)

$publishPath = "Auto_Updater\bin\Release\net9.0-windows\win-x64\publish"
$outputPath = "releases"
$installerName = "Auto_Updater-Setup-v$Version.exe"

# Create releases directory if it doesn't exist
if (!(Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
}

# Copy the executable as the installer (for demo purposes)
# In production, you'd use Inno Setup, WiX, or similar
Write-Host "Creating installer for version $Version..."
Copy-Item "$publishPath\Auto_Updater.exe" "$outputPath\$installerName" -Force

Write-Host "✓ Installer created: $outputPath\$installerName"
Write-Host ""
Write-Host "Calculating SHA256 checksum..."

$hash = (Get-FileHash "$outputPath\$installerName" -Algorithm SHA256).Hash
Write-Host "SHA256: $hash"
Write-Host ""
Write-Host "File size: $((Get-Item "$outputPath\$installerName").Length / 1MB) MB"

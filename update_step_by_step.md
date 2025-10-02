# Update Step-by-Step Guide

This guide explains how to release new versions of the auto-updater system.

---

## 📋 Table of Contents

1. [Release New Version of MainApp](#1-release-new-version-of-mainapp)
2. [Release New Version of Updater (Launcher)](#2-release-new-version-of-updater-launcher)
3. [Release Both Together](#3-release-both-together)
4. [Quick Reference Commands](#4-quick-reference-commands)

---

## 1. Release New Version of MainApp

This is the most common scenario - updating your main application while keeping the launcher the same.

### Step 1.1: Update MainApp Code

Edit your application in `MainApp/MainWindow.xaml` and `MainApp/MainWindow.xaml.cs`:

```csharp
// Example: Change the UI
<TextBlock Text="Version 1.0.3"  // Update version text
           FontSize="20"
           Foreground="Blue"      // Change color
           .../>
```

### Step 1.2: Update Version Number

Edit `MainApp/MainApp.csproj`:

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    ...
    <Version>1.0.3.0</Version>              <!-- Change this -->
    <AssemblyVersion>1.0.3.0</AssemblyVersion>  <!-- Change this -->
    <FileVersion>1.0.3.0</FileVersion>      <!-- Change this -->
</PropertyGroup>
```

### Step 1.3: Update Title (Optional)

Edit `MainApp/MainWindow.xaml`:

```xml
<Window ...
        Title="Main Application - v1.0.3"  <!-- Update title -->
        ...>
```

### Step 1.4: Build Both Projects

```bash
# Build Launcher (no changes, but needed for complete package)
cd Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/v1.0.3

# Build MainApp (your new version)
cd ../MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/v1.0.3/MainApp
```

### Step 1.5: Copy Configuration

```bash
# Copy config.ini to release folder
cp ../config.ini ../releases/v1.0.3/
```

### Step 1.6: Create ZIP Package

```powershell
# PowerShell
Compress-Archive -Path 'releases\v1.0.3\*' -DestinationPath 'releases\Auto_Updater-v1.0.3.zip' -Force
```

Or using Bash:
```bash
cd releases
zip -r Auto_Updater-v1.0.3.zip v1.0.3/*
```

### Step 1.7: Calculate Checksum

```powershell
# PowerShell
Get-FileHash 'releases\Auto_Updater-v1.0.3.zip' -Algorithm SHA256
```

**Copy the hash value** (e.g., `ABC123...`)

### Step 1.8: Update update.xml

Edit `update.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.3.0</version>  <!-- New version -->
    <url>https://github.com/lwsvincent/Auto_Updater/releases/download/v1.0.3/Auto_Updater-v1.0.3.zip</url>
    <changelog>https://github.com/lwsvincent/Auto_Updater/releases/tag/v1.0.3</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">PASTE_YOUR_HASH_HERE</checksum>  <!-- Paste hash from Step 1.7 -->
</item>
```

### Step 1.9: Create GitHub Release

```bash
# Commit changes first
git add -A
git commit -m "Release v1.0.3 - Updated MainApp with new features"
git push

# Create GitHub release
gh release create v1.0.3 \
  "releases/Auto_Updater-v1.0.3.zip" \
  --title "v1.0.3 - MainApp Update" \
  --notes "## Changes
- Updated MainApp UI
- Added new features
- Bug fixes

### Installation
Download Auto_Updater-v1.0.3.zip and extract."
```

### Step 1.10: Push update.xml Changes

```bash
# Commit and push update.xml (this triggers the update for users)
git add update.xml
git commit -m "Update manifest to v1.0.3"
git push
```

✅ **Done!** Users running v1.0.2 will now see an update notification when they run Launcher.exe.

---

## 2. Release New Version of Updater (Launcher)

This is less common - only when you need to update the launcher itself.

### Step 2.1: Update Launcher Code

Edit `Launcher/MainWindow.xaml` or `Launcher/MainWindow.xaml.cs`:

```csharp
// Example: Change loading message
UpdateStatus("Checking for updates...", "Connecting to update server v2");
```

### Step 2.2: Update Version in Launcher Project

Edit `Launcher/Launcher.csproj`:

```xml
<PropertyGroup>
    <Version>1.1.0.0</Version>
    <AssemblyVersion>1.1.0.0</AssemblyVersion>
    <FileVersion>1.1.0.0</FileVersion>
</PropertyGroup>
```

### Step 2.3: Update MainApp Version Too

**Important:** When updating the launcher, also update MainApp version (even if no changes):

Edit `MainApp/MainApp.csproj`:

```xml
<PropertyGroup>
    <Version>1.1.0.0</Version>  <!-- Match launcher version -->
    <AssemblyVersion>1.1.0.0</AssemblyVersion>
    <FileVersion>1.1.0.0</FileVersion>
</PropertyGroup>
```

### Step 2.4: Build Both Projects

```bash
# Build Launcher (with changes)
cd Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/v1.1.0

# Build MainApp
cd ../MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/v1.1.0/MainApp

# Copy config
cp ../config.ini ../releases/v1.1.0/
```

### Step 2.5: Create Package and Follow Steps 1.6 - 1.10

Same as MainApp release (create ZIP, checksum, update XML, create GitHub release, push).

⚠️ **Note:** When launcher updates itself, it will download the new ZIP, extract it, and restart.

---

## 3. Release Both Together

When you update both Launcher and MainApp at the same time.

### Step 3.1: Update Both Version Numbers

**Launcher/Launcher.csproj:**
```xml
<Version>1.2.0.0</Version>
<AssemblyVersion>1.2.0.0</AssemblyVersion>
<FileVersion>1.2.0.0</FileVersion>
```

**MainApp/MainApp.csproj:**
```xml
<Version>1.2.0.0</Version>
<AssemblyVersion>1.2.0.0</AssemblyVersion>
<FileVersion>1.2.0.0</FileVersion>
```

### Step 3.2: Make Your Code Changes

Edit both projects as needed.

### Step 3.3: Follow Steps 1.4 - 1.10

Build, package, checksum, update XML, create release, push.

---

## 4. Quick Reference Commands

### Complete Release Script (Copy & Paste)

Replace `VERSION` with your new version (e.g., `1.0.3`):

```bash
# Set version variable
VERSION="1.0.3"

# 1. Build Launcher
cd Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/v${VERSION}

# 2. Build MainApp
cd ../MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/v${VERSION}/MainApp

# 3. Copy config
cd ..
cp config.ini releases/v${VERSION}/

# 4. Create ZIP (PowerShell)
powershell -Command "Compress-Archive -Path 'releases\v${VERSION}\*' -DestinationPath 'releases\Auto_Updater-v${VERSION}.zip' -Force"

# 5. Get checksum (PowerShell)
powershell -Command "(Get-FileHash 'releases\Auto_Updater-v${VERSION}.zip' -Algorithm SHA256).Hash"

# 6. Edit update.xml manually with new version and checksum

# 7. Commit and push
git add -A
git commit -m "Release v${VERSION}"
git push

# 8. Create GitHub release
gh release create v${VERSION} \
  "releases/Auto_Updater-v${VERSION}.zip" \
  --title "v${VERSION}" \
  --notes "Release v${VERSION}"

# 9. Push update.xml
git add update.xml
git commit -m "Update manifest to v${VERSION}"
git push
```

---

## 5. Testing Updates

### Test Locally Before Release

1. **Build new version** but don't create GitHub release yet
2. **Copy `releases/vX.X.X/` folder** to a test location
3. **Run Launcher.exe** from test location
4. **Verify MainApp launches correctly**
5. **Test all features**

### Test Update Process

1. **Keep old version** in one folder (e.g., v1.0.2)
2. **Create GitHub release** with new version (e.g., v1.0.3)
3. **Update update.xml** and push
4. **Run old version's Launcher.exe**
5. **Verify update notification appears**
6. **Click Update** and verify download/install works
7. **Verify new version runs correctly**

---

## 6. Versioning Strategy

### Semantic Versioning (Recommended)

Format: `MAJOR.MINOR.PATCH.BUILD`

**Examples:**
- `1.0.0.0` - Initial release
- `1.0.1.0` - Bug fix (patch)
- `1.1.0.0` - New features (minor)
- `2.0.0.0` - Breaking changes (major)

### When to Increment

- **MAJOR (1.x.x → 2.x.x)**: Breaking changes, major refactor
- **MINOR (1.0.x → 1.1.x)**: New features, non-breaking changes
- **PATCH (1.0.0 → 1.0.1)**: Bug fixes only
- **BUILD (1.0.0.0 → 1.0.0.1)**: Internal builds (optional)

---

## 7. Common Scenarios

### Scenario A: Fix a Bug in MainApp

```bash
VERSION="1.0.3"
# 1. Fix bug in MainApp code
# 2. Update MainApp version to 1.0.3
# 3. Build and release (steps 1.4-1.10)
```

### Scenario B: Add New Feature to MainApp

```bash
VERSION="1.1.0"
# 1. Add feature in MainApp code
# 2. Update MainApp version to 1.1.0
# 3. Build and release (steps 1.4-1.10)
```

### Scenario C: Fix Launcher Bug

```bash
VERSION="1.0.3"
# 1. Fix bug in Launcher code
# 2. Update both Launcher and MainApp to 1.0.3
# 3. Build and release (steps 2.4-2.5)
```

### Scenario D: Major Update

```bash
VERSION="2.0.0"
# 1. Make breaking changes to both
# 2. Update both to 2.0.0
# 3. Build and release (steps 3.1-3.3)
# 4. Consider setting mandatory=true in update.xml
```

---

## 8. Troubleshooting

### Issue: Users Not Seeing Update

**Check:**
1. Is `update.xml` pushed to GitHub? Check: https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/update.xml
2. Is version in `update.xml` higher than user's current version?
3. Is GitHub release created with correct tag?
4. Is ZIP file uploaded to the release?

### Issue: Update Download Fails

**Check:**
1. Is the ZIP file URL correct in `update.xml`?
2. Is the GitHub release public?
3. Is the checksum correct?

### Issue: Update Installs but Doesn't Work

**Check:**
1. Did you include all DLLs in the ZIP?
2. Is `config.ini` in the ZIP?
3. Is folder structure correct? (Launcher.exe in root, MainApp.exe in MainApp/)
4. Did you test the ZIP manually before releasing?

---

## 9. Rollback

### If You Need to Rollback

1. **Edit `update.xml`** to point to previous version:

```xml
<item>
    <version>1.0.2.0</version>  <!-- Back to old version -->
    <url>https://github.com/lwsvincent/Auto_Updater/releases/download/v1.0.2/Auto_Updater-v1.0.2-fixed.zip</url>
    <changelog>https://github.com/lwsvincent/Auto_Updater/releases/tag/v1.0.2</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">DF9F03C8D869C1747BD5CA80E73491C8483C8F2699F2C638874CA20F2E29E76C</checksum>
</item>
```

2. **Commit and push**:

```bash
git add update.xml
git commit -m "Rollback to v1.0.2 due to critical bug"
git push
```

3. Users will be offered to "update" back to the old version

---

## 10. Best Practices

### ✅ DO:
- Always test locally before releasing
- Always calculate and verify checksum
- Keep old releases available on GitHub
- Use semantic versioning
- Write clear release notes
- Test the update process from previous version
- Backup before major updates

### ❌ DON'T:
- Don't delete old GitHub releases (users might need to rollback)
- Don't forget to update `update.xml` after creating release
- Don't skip checksum validation
- Don't release without testing
- Don't change version format (stay consistent)

---

## 11. Automation (Advanced)

### PowerShell Release Script

Save as `release.ps1`:

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

Write-Host "Building version $Version..." -ForegroundColor Green

# Build projects
Set-Location Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o "../releases/v$Version"

Set-Location ../MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o "../releases/v$Version/MainApp"

Set-Location ..
Copy-Item config.ini "releases/v$Version/"

# Create ZIP
Compress-Archive -Path "releases\v$Version\*" -DestinationPath "releases\Auto_Updater-v$Version.zip" -Force

# Get checksum
$hash = (Get-FileHash "releases\Auto_Updater-v$Version.zip" -Algorithm SHA256).Hash
Write-Host "SHA256: $hash" -ForegroundColor Yellow
Write-Host "Please update update.xml with this checksum!" -ForegroundColor Yellow

# Commit
git add -A
git commit -m "Release v$Version"
git push

# Create release
gh release create "v$Version" "releases/Auto_Updater-v$Version.zip" --title "v$Version" --notes "Release v$Version"

Write-Host "Done! Don't forget to update update.xml and push!" -ForegroundColor Green
```

**Usage:**
```powershell
.\release.ps1 -Version "1.0.3"
```

---

## 📝 Summary Checklist

### For Every Release:

- [ ] Update version numbers in `.csproj` files
- [ ] Make code changes
- [ ] Build both Launcher and MainApp
- [ ] Copy `config.ini` to release folder
- [ ] Create ZIP package
- [ ] Calculate SHA256 checksum
- [ ] Update `update.xml` with new version, URL, and checksum
- [ ] Commit code changes
- [ ] Create GitHub release with ZIP file
- [ ] Push `update.xml` changes to trigger update
- [ ] Test update process from previous version

---

**Need Help?**
- Check the [README.md](README.md) for architecture details
- View [SUMMARY.md](SUMMARY.md) for project overview
- See example releases: https://github.com/lwsvincent/Auto_Updater/releases

🤖 Generated with [Claude Code](https://claude.com/claude-code)

# Update Step-by-Step Guide (NEW: Separate Update Channels)

This guide explains how to release new versions with **independent update channels** for Launcher and MainApp.

---

## 📋 Table of Contents

1. [Understanding the New System](#1-understanding-the-new-system)
2. [Release New Version of Launcher Only](#2-release-new-version-of-launcher-only)
3. [Release New Version of MainApp Only](#3-release-new-version-of-mainapp-only)
4. [Release Both Together](#4-release-both-together)
5. [Use Launcher with Your Own App](#5-use-launcher-with-your-own-app)
6. [Quick Reference Commands](#6-quick-reference-commands)

---

## 1. Understanding the New System

### 🎯 Key Concepts

**OLD System (v1.0.0-1.0.2):**
- Single `update.xml` for everything
- Update anything → Download entire package
- Not reusable for other projects

**NEW System (v1.0.3+):**
- `launcher-update.xml` → Updates Launcher only
- `app-update.xml` → Updates MainApp only
- Can update each independently
- **Launcher is reusable for any project!**

### 📁 File Structure

```
GitHub Repository:
├── launcher-update.xml    # Launcher version info
├── app-update.xml         # MainApp version info
└── config.ini             # Configuration

GitHub Releases:
├── launcher-vX.X.X/       # Launcher-only releases
│   └── Launcher-vX.X.X.zip
│       └── Launcher.exe + dependencies
│
└── app-vX.X.X/            # MainApp-only releases
    └── MainApp-vX.X.X.zip
        └── MainApp/ folder
```

### 🔄 Update Flow

```
User runs Launcher.exe
    ↓
1. Check launcher-update.xml
    ↓ (if update available)
   Download Launcher ZIP → Extract → Restart
    ↓
2. Check app-update.xml
    ↓ (if update available)
   Download MainApp ZIP → Extract
    ↓
3. Launch MainApp
```

---

## 2. Release New Version of Launcher Only

**When:** You fixed a bug in Launcher or added new features to the updater itself.

### Step 2.1: Update Launcher Version

Edit `Launcher/Launcher.csproj`:

```xml
<PropertyGroup>
    <Version>1.0.3.0</Version>              <!-- Change this -->
    <AssemblyVersion>1.0.3.0</AssemblyVersion>
    <FileVersion>1.0.3.0</FileVersion>
</PropertyGroup>
```

### Step 2.2: Make Your Changes

Edit Launcher code in `Launcher/MainWindow.xaml.cs` or `MainWindow.xaml`.

### Step 2.3: Build Launcher

```bash
cd Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/launcher-v1.0.3
```

### Step 2.4: Create Launcher ZIP

**Important:** Launcher ZIP contains ONLY Launcher.exe and its dependencies (no MainApp).

```powershell
# PowerShell
Compress-Archive -Path 'releases\launcher-v1.0.3\*' -DestinationPath 'releases\Launcher-v1.0.3.zip' -Force
```

### Step 2.5: Calculate Checksum

```powershell
Get-FileHash 'releases\Launcher-v1.0.3.zip' -Algorithm SHA256
```

Copy the hash (e.g., `ABC123...`)

### Step 2.6: Update launcher-update.xml

Edit `launcher-update.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.3.0</version>
    <url>https://github.com/lwsvincent/Auto_Updater/releases/download/launcher-v1.0.3/Launcher-v1.0.3.zip</url>
    <changelog>https://github.com/lwsvincent/Auto_Updater/releases/tag/launcher-v1.0.3</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">PASTE_HASH_HERE</checksum>
</item>
```

### Step 2.7: Create GitHub Release

```bash
gh release create launcher-v1.0.3 \
  "releases/Launcher-v1.0.3.zip" \
  --title "Launcher v1.0.3" \
  --notes "## Launcher Update v1.0.3

- Fixed bug in update checker
- Improved error handling

**Note:** This updates only the Launcher. MainApp is not affected."
```

### Step 2.8: Commit and Push

**Do NOT commit yet** - wait for user instruction.

When ready:
```bash
git add launcher-update.xml Launcher/
git commit -m "Release Launcher v1.0.3 - Bug fixes"
git push
```

✅ **Done!** Users will get Launcher update notification. After update, Launcher restarts and launches their existing MainApp.

---

## 3. Release New Version of MainApp Only

**When:** You added features or fixed bugs in MainApp. Launcher stays the same.

### Step 3.1: Update MainApp Version

Edit `MainApp/MainApp.csproj`:

```xml
<PropertyGroup>
    <Version>1.0.3.0</Version>              <!-- Change this -->
    <AssemblyVersion>1.0.3.0</AssemblyVersion>
    <FileVersion>1.0.3.0</FileVersion>
</PropertyGroup>
```

### Step 3.2: Update MainApp UI (Optional)

Edit `MainApp/MainWindow.xaml`:

```xml
Title="Main Application - v1.0.3"  <!-- Update title -->

<TextBlock Text="Version 1.0.3"    <!-- Update version display -->
```

### Step 3.3: Make Your Code Changes

Edit `MainApp/MainWindow.xaml.cs` or other files as needed.

### Step 3.4: Build MainApp

```bash
cd MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/app-v1.0.3/MainApp
```

### Step 3.5: Create MainApp ZIP

**Important:** ZIP contains ONLY the MainApp folder (not Launcher).

```powershell
# Create the folder structure first
New-Item -ItemType Directory -Path "releases\app-v1.0.3-package" -Force
Copy-Item -Recurse "releases\app-v1.0.3\MainApp" "releases\app-v1.0.3-package\"

# Create ZIP
Compress-Archive -Path 'releases\app-v1.0.3-package\*' -DestinationPath 'releases\MainApp-v1.0.3.zip' -Force
```

**ZIP Structure:**
```
MainApp-v1.0.3.zip
└── MainApp/
    ├── MainApp.exe
    └── (dependencies)
```

### Step 3.6: Calculate Checksum

```powershell
Get-FileHash 'releases\MainApp-v1.0.3.zip' -Algorithm SHA256
```

Copy the hash.

### Step 3.7: Update app-update.xml

Edit `app-update.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.3.0</version>
    <url>https://github.com/lwsvincent/Auto_Updater/releases/download/app-v1.0.3/MainApp-v1.0.3.zip</url>
    <changelog>https://github.com/lwsvincent/Auto_Updater/releases/tag/app-v1.0.3</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">PASTE_HASH_HERE</checksum>
</item>
```

### Step 3.8: Create GitHub Release

```bash
gh release create app-v1.0.3 \
  "releases/MainApp-v1.0.3.zip" \
  --title "MainApp v1.0.3" \
  --notes "## MainApp Update v1.0.3

### New Features
- Added dark mode
- Improved performance

### Bug Fixes
- Fixed crash on startup

**Note:** This updates only the MainApp. Launcher is not affected."
```

### Step 3.9: Commit and Push

**Do NOT commit yet** - wait for user instruction.

When ready:
```bash
git add app-update.xml MainApp/
git commit -m "Release MainApp v1.0.3 - New features"
git push
```

✅ **Done!** Users will get MainApp update notification. Launcher stays the same.

---

## 4. Release Both Together

**When:** Both Launcher and MainApp need updates.

### Step 4.1: Update Both Versions

**Launcher/Launcher.csproj:**
```xml
<Version>1.0.3.0</Version>
```

**MainApp/MainApp.csproj:**
```xml
<Version>1.0.3.0</Version>
```

### Step 4.2: Make Changes to Both Projects

Edit both Launcher and MainApp code as needed.

### Step 4.3: Build Both

```bash
# Build Launcher
cd Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/launcher-v1.0.3

# Build MainApp
cd ../MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/app-v1.0.3/MainApp
```

### Step 4.4: Create TWO Separate ZIPs

```powershell
# Launcher ZIP
Compress-Archive -Path 'releases\launcher-v1.0.3\*' -DestinationPath 'releases\Launcher-v1.0.3.zip' -Force

# MainApp ZIP
New-Item -ItemType Directory -Path "releases\app-v1.0.3-package" -Force
Copy-Item -Recurse "releases\app-v1.0.3\MainApp" "releases\app-v1.0.3-package\"
Compress-Archive -Path 'releases\app-v1.0.3-package\*' -DestinationPath 'releases\MainApp-v1.0.3.zip' -Force
```

### Step 4.5: Calculate Both Checksums

```powershell
Get-FileHash 'releases\Launcher-v1.0.3.zip' -Algorithm SHA256
Get-FileHash 'releases\MainApp-v1.0.3.zip' -Algorithm SHA256
```

### Step 4.6: Update BOTH XML Files

**launcher-update.xml:**
```xml
<version>1.0.3.0</version>
<url>.../launcher-v1.0.3/Launcher-v1.0.3.zip</url>
<checksum>LAUNCHER_HASH</checksum>
```

**app-update.xml:**
```xml
<version>1.0.3.0</version>
<url>.../app-v1.0.3/MainApp-v1.0.3.zip</url>
<checksum>MAINAPP_HASH</checksum>
```

### Step 4.7: Create TWO GitHub Releases

```bash
# Launcher release
gh release create launcher-v1.0.3 \
  "releases/Launcher-v1.0.3.zip" \
  --title "Launcher v1.0.3" \
  --notes "Launcher update v1.0.3"

# MainApp release
gh release create app-v1.0.3 \
  "releases/MainApp-v1.0.3.zip" \
  --title "MainApp v1.0.3" \
  --notes "MainApp update v1.0.3"
```

### Step 4.8: Commit and Push

**Do NOT commit yet** - wait for user instruction.

When ready:
```bash
git add launcher-update.xml app-update.xml Launcher/ MainApp/
git commit -m "Release v1.0.3 - Both Launcher and MainApp updated"
git push
```

✅ **Done!** Users will get both updates. Launcher updates first, restarts, then MainApp updates.

---

## 5. Use Launcher with Your Own App

**Scenario:** You want to use the Auto_Updater Launcher with your own application.

### Step 5.1: Copy Launcher Files

From `releases/launcher-vX.X.X/`, copy:
- `Launcher.exe`
- All `.dll` files

To your project folder:
```
YourProject/
├── Launcher.exe
├── Autoupdater.NET.Official.dll
├── ini-parser.dll
└── (other DLLs)
```

### Step 5.2: Create Your App Folder

```
YourProject/
├── Launcher.exe
├── *.dll
└── YourApp/              ← Create this
    └── YourApp.exe       ← Your application
```

### Step 5.3: Create config.ini

Create `config.ini` next to Launcher.exe:

```ini
[Updater]
LauncherUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/launcher-update.xml
AppUpdateUrl=https://raw.githubusercontent.com/YOUR_USERNAME/YOUR_REPO/master/your-app-update.xml
MainAppPath=YourApp\YourApp.exe
AutoCheckLauncherUpdate=true
AutoCheckAppUpdate=true
ShowUpdateUI=true
MandatoryLauncherUpdate=false
MandatoryAppUpdate=false

[Application]
AppName=Your Application Name
AppVersion=1.0.0
LauncherVersion=1.0.2
```

**Change:**
- `YOUR_USERNAME/YOUR_REPO` → Your GitHub repo
- `YourApp\YourApp.exe` → Path to your app
- `Your Application Name` → Your app's name

### Step 5.4: Create your-app-update.xml in Your Repo

In your GitHub repository, create `your-app-update.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.0.0</version>
    <url>https://github.com/YOUR_USERNAME/YOUR_REPO/releases/download/v1.0.0/YourApp-v1.0.0.zip</url>
    <changelog>https://github.com/YOUR_USERNAME/YOUR_REPO/releases/tag/v1.0.0</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">YOUR_CHECKSUM</checksum>
</item>
```

### Step 5.5: Create Your App Release

```bash
# Package your app
Compress-Archive -Path 'YourApp\*' -DestinationPath 'YourApp-v1.0.0.zip'

# Get checksum
Get-FileHash 'YourApp-v1.0.0.zip' -Algorithm SHA256

# Update your-app-update.xml with checksum

# Create GitHub release
gh release create v1.0.0 YourApp-v1.0.0.zip --title "v1.0.0" --notes "Initial release"
```

### Step 5.6: Test

1. Run `Launcher.exe`
2. It checks for updates
3. Launches `YourApp/YourApp.exe`

✅ **Your app now has auto-update!**

### Step 5.7: Release New Version of Your App

```bash
# 1. Update YourApp version in your project
# 2. Build your app
# 3. Create new ZIP
Compress-Archive -Path 'YourApp\*' -DestinationPath 'YourApp-v1.0.1.zip'

# 4. Get checksum
Get-FileHash 'YourApp-v1.0.1.zip' -Algorithm SHA256

# 5. Update your-app-update.xml
# <version>1.0.1.0</version>
# <url>.../v1.0.1/YourApp-v1.0.1.zip</url>
# <checksum>NEW_HASH</checksum>

# 6. Create release
gh release create v1.0.1 YourApp-v1.0.1.zip

# 7. Push your-app-update.xml
git add your-app-update.xml
git commit -m "Update to v1.0.1"
git push
```

✅ **Users automatically get the update!**

---

## 6. Quick Reference Commands

### Update Launcher

```bash
# Set version
VERSION="1.0.3"

# Build
cd Launcher
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/launcher-v${VERSION}

# Package
cd ..
powershell -Command "Compress-Archive -Path 'releases\launcher-v${VERSION}\*' -DestinationPath 'releases\Launcher-v${VERSION}.zip' -Force"

# Checksum
powershell -Command "(Get-FileHash 'releases\Launcher-v${VERSION}.zip' -Algorithm SHA256).Hash"

# Update launcher-update.xml with version, URL, and checksum

# Release
gh release create launcher-v${VERSION} "releases/Launcher-v${VERSION}.zip" --title "Launcher v${VERSION}"
```

### Update MainApp

```bash
# Set version
VERSION="1.0.3"

# Build
cd MainApp
dotnet publish -c Release -r win-x64 --no-self-contained -o ../releases/app-v${VERSION}/MainApp

# Package
cd ..
powershell -Command "New-Item -ItemType Directory -Path 'releases\app-v${VERSION}-package' -Force"
powershell -Command "Copy-Item -Recurse 'releases\app-v${VERSION}\MainApp' 'releases\app-v${VERSION}-package\'"
powershell -Command "Compress-Archive -Path 'releases\app-v${VERSION}-package\*' -DestinationPath 'releases\MainApp-v${VERSION}.zip' -Force"

# Checksum
powershell -Command "(Get-FileHash 'releases\MainApp-v${VERSION}.zip' -Algorithm SHA256).Hash"

# Update app-update.xml with version, URL, and checksum

# Release
gh release create app-v${VERSION} "releases/MainApp-v${VERSION}.zip" --title "MainApp v${VERSION}"
```

---

## 7. Testing Updates

### Test Launcher Update Locally

1. Build Launcher v1.0.2
2. Build Launcher v1.0.3
3. Run v1.0.2
4. Point to test server with v1.0.3 update.xml
5. Verify update works

### Test MainApp Update Locally

1. Build MainApp v1.0.2
2. Build MainApp v1.0.3
3. Package v1.0.3
4. Create local test server with update.xml
5. Run Launcher → Verify MainApp updates

### Test Complete Installation

1. Create complete package (Launcher + MainApp + config.ini)
2. Extract to test folder
3. Run Launcher.exe
4. Verify it launches MainApp
5. Trigger updates
6. Verify both update independently

---

## 8. Troubleshooting

### Launcher Not Updating

**Check:**
- Is `launcher-update.xml` pushed to GitHub?
- Is version number higher than current?
- Is GitHub release created with correct tag?
- Is ZIP file uploaded?
- Is checksum correct?

### MainApp Not Updating

**Check:**
- Is `app-update.xml` pushed to GitHub?
- Is version number higher than current?
- Is `AppUpdateUrl` correct in config.ini?
- Is MainApp ZIP structured correctly? (Should contain `MainApp/` folder)

### Both Updates Triggered Unexpectedly

**Check:**
- Versions in XML files
- Versions in `.csproj` files
- Make sure versions match what you intended

---

## 9. Best Practices

### ✅ DO:

- Keep Launcher and MainApp versions independent
- Update only what changed (Launcher OR MainApp, not always both)
- Test updates locally before releasing
- Use semantic versioning (Major.Minor.Patch)
- Write clear release notes
- Keep old releases available for rollback

### ❌ DON'T:

- Don't update both if only one changed
- Don't forget to update XML files after creating releases
- Don't skip checksum validation
- Don't delete old GitHub releases
- Don't use same release tag for both (use `launcher-vX.X.X` and `app-vX.X.X`)

---

## 10. Summary Checklist

### For Launcher Update:
- [ ] Update version in `Launcher/Launcher.csproj`
- [ ] Make code changes
- [ ] Build Launcher
- [ ] Create `Launcher-vX.X.X.zip`
- [ ] Calculate SHA256
- [ ] Update `launcher-update.xml`
- [ ] Create GitHub release `launcher-vX.X.X`
- [ ] Commit and push (when user approves)

### For MainApp Update:
- [ ] Update version in `MainApp/MainApp.csproj`
- [ ] Make code changes
- [ ] Build MainApp
- [ ] Create `MainApp-vX.X.X.zip` (with MainApp/ folder)
- [ ] Calculate SHA256
- [ ] Update `app-update.xml`
- [ ] Create GitHub release `app-vX.X.X`
- [ ] Commit and push (when user approves)

### For Using with Your Own App:
- [ ] Copy Launcher.exe + DLLs
- [ ] Create your app folder
- [ ] Create config.ini pointing to your app
- [ ] Create your-app-update.xml in your repo
- [ ] Create GitHub release with your app ZIP
- [ ] Test: Run Launcher → Launches your app
- [ ] Test: Create v1.0.1 → Users get update

---

**Need Help?**
- See [ARCHITECTURE.md](ARCHITECTURE.md) for system design
- See [REUSE_LAUNCHER_GUIDE.md](REUSE_LAUNCHER_GUIDE.md) for detailed reuse guide
- See [README.md](README.md) for project overview

🤖 Generated with [Claude Code](https://claude.com/claude-code)

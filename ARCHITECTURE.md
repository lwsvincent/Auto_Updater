# Architecture: Separate Update Channels

## 🎯 Overview

The Auto_Updater now supports **independent update channels** for Launcher and MainApp.

```
┌─────────────────────────────────────────────┐
│            Launcher.exe v1.0.2              │
│  ┌───────────────────────────────────────┐  │
│  │ STEP 1: Check launcher-update.xml    │  │
│  │  ↓                                    │  │
│  │ Is Launcher update available?         │  │
│  │  ↓ YES                    ↓ NO        │  │
│  │ Update Launcher      STEP 2           │  │
│  │  & Restart                             │  │
│  │                                        │  │
│  │ STEP 2: Check app-update.xml          │  │
│  │  ↓                                    │  │
│  │ Is MainApp update available?          │  │
│  │  ↓ YES                    ↓ NO        │  │
│  │ Update MainApp       Launch MainApp   │  │
│  │  & Launch                              │  │
│  └───────────────────────────────────────┘  │
└─────────────────────────────────────────────┘
```

---

## 📁 File Structure

```
GitHub Repository:
├── launcher-update.xml    # Launcher version manifest
├── app-update.xml         # MainApp version manifest
└── config.ini             # Configuration (in release package)

GitHub Releases:
├── launcher-v1.0.2/       # Launcher-only release
│   └── Launcher-v1.0.2.zip
│       └── Launcher.exe (+ dependencies)
│
└── app-v1.0.2/            # MainApp-only release
    └── MainApp-v1.0.2.zip
        └── MainApp/ (MainApp.exe + dependencies)
```

---

## ⚙️ Configuration (config.ini)

```ini
[Updater]
# SEPARATE update URLs for launcher and app
LauncherUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/launcher-update.xml
AppUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/app-update.xml

# Main application path
MainAppPath=MainApp\MainApp.exe

# Control each update channel independently
AutoCheckLauncherUpdate=true    # Check for launcher updates
AutoCheckAppUpdate=true          # Check for app updates
ShowUpdateUI=true
MandatoryLauncherUpdate=false    # Force launcher updates
MandatoryAppUpdate=false         # Force app updates

[Application]
AppName=Auto Updater Demo
AppVersion=1.0.2        # MainApp version
LauncherVersion=1.0.2   # Launcher version
```

---

## 📄 Update Manifests

### launcher-update.xml

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.2.0</version>
    <url>https://github.com/lwsvincent/Auto_Updater/releases/download/launcher-v1.0.2/Launcher-v1.0.2.zip</url>
    <changelog>https://github.com/lwsvincent/Auto_Updater/releases/tag/launcher-v1.0.2</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">HASH_HERE</checksum>
</item>
```

### app-update.xml

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.2.0</version>
    <url>https://github.com/lwsvincent/Auto_Updater/releases/download/app-v1.0.2/MainApp-v1.0.2.zip</url>
    <changelog>https://github.com/lwsvincent/Auto_Updater/releases/tag/app-v1.0.2</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">HASH_HERE</checksum>
</item>
```

---

## 🔄 Update Flow

### Complete Flow

```
START
  ↓
[Load config.ini]
  ↓
[AutoCheckLauncherUpdate?]
  ↓ YES                           ↓ NO
[Fetch launcher-update.xml]   [SKIP TO APP UPDATE]
  ↓
[Compare Launcher versions]
  ↓
[Launcher update available?]
  ↓ YES                           ↓ NO
[Show launcher update dialog]  [Continue to app update]
  ↓
[User: Update / Skip]
  ↓ Update                       ↓ Skip
[Download Launcher ZIP]       [Continue to app update]
  ↓
[Extract & Replace Launcher.exe]
  ↓
[Restart Launcher.exe]
  ↓
  ├─────────────────────────────┤
  ↓
[AutoCheckAppUpdate?]
  ↓ YES                           ↓ NO
[Fetch app-update.xml]         [Launch MainApp]
  ↓
[Compare MainApp versions]
  ↓
[App update available?]
  ↓ YES                           ↓ NO
[Show app update dialog]       [Launch MainApp]
  ↓
[User: Update / Skip]
  ↓ Update                       ↓ Skip
[Download MainApp ZIP]         [Launch MainApp]
  ↓
[Extract & Replace MainApp/]
  ↓
[Launch MainApp]
```

---

## 🚀 Release Scenarios

### Scenario 1: Update Launcher Only

**What changed:** Fixed a bug in Launcher

**Steps:**
1. Update `Launcher/Launcher.csproj` version to 1.0.3
2. Build Launcher
3. Create `Launcher-v1.0.3.zip` (contains only Launcher.exe + dependencies)
4. Upload to GitHub as release `launcher-v1.0.3`
5. Update `launcher-update.xml`:
   ```xml
   <version>1.0.3.0</version>
   <url>.../launcher-v1.0.3/Launcher-v1.0.3.zip</url>
   ```
6. Push `launcher-update.xml` to GitHub

**Result:**
- Users get Launcher update dialog
- Only Launcher.exe is replaced
- MainApp.exe stays the same
- After restart, updated Launcher launches existing MainApp

---

### Scenario 2: Update MainApp Only

**What changed:** Added new feature to MainApp

**Steps:**
1. Update `MainApp/MainApp.csproj` version to 1.0.3
2. Build MainApp
3. Create `MainApp-v1.0.3.zip` (contains only MainApp/ folder)
4. Upload to GitHub as release `app-v1.0.3`
5. Update `app-update.xml`:
   ```xml
   <version>1.0.3.0</version>
   <url>.../app-v1.0.3/MainApp-v1.0.3.zip</url>
   ```
6. Push `app-update.xml` to GitHub

**Result:**
- Launcher checks itself: No update
- Launcher checks MainApp: Update available!
- Users get MainApp update dialog
- Only MainApp/ folder is replaced
- Launcher.exe stays the same
- After update, Launcher launches new MainApp

---

### Scenario 3: Update Both

**What changed:** Both Launcher and MainApp need updates

**Steps:**
1. Update both project versions to 1.0.3
2. Build both
3. Create TWO separate releases:
   - `launcher-v1.0.3` with `Launcher-v1.0.3.zip`
   - `app-v1.0.3` with `MainApp-v1.0.3.zip`
4. Update BOTH XML files
5. Push both to GitHub

**Result:**
- Launcher checks itself: Update! → Downloads & restarts
- After restart, checks MainApp: Update! → Downloads
- Both are updated independently

---

## 🎯 Benefits

### 1. **Independent Updates**
- Update Launcher without touching MainApp
- Update MainApp without re-downloading Launcher
- Smaller download sizes

### 2. **Multiple Projects**
- One Launcher can serve multiple MainApps
- Just change `MainAppPath` and `AppUpdateUrl` in config.ini
- Each MainApp has its own update channel

### 3. **Reusable Launcher**
```
Your Projects:
├── ProjectA/
│   ├── Launcher.exe (shared)
│   ├── config.ini → AppUpdateUrl=.../projectA-update.xml
│   └── MainApp/ (ProjectA specific)
│
├── ProjectB/
│   ├── Launcher.exe (shared, same version)
│   ├── config.ini → AppUpdateUrl=.../projectB-update.xml
│   └── MainApp/ (ProjectB specific)
```

### 4. **Flexible Deployment**
- Update Launcher globally for all projects
- Update each MainApp independently
- Users get updates for their specific app

---

## 📦 Package Structure

### Launcher Release Package (launcher-vX.X.X)
```
Launcher-vX.X.X.zip
├── Launcher.exe
├── Autoupdater.NET.Official.dll
├── ini-parser.dll
└── (other launcher dependencies)
```

### MainApp Release Package (app-vX.X.X)
```
MainApp-vX.X.X.zip
└── MainApp/
    ├── MainApp.exe
    └── (app-specific dependencies)
```

### Complete Installation Package (for first-time users)
```
Auto_Updater-Complete-vX.X.X.zip
├── Launcher.exe
├── config.ini
├── (launcher dependencies)
└── MainApp/
    ├── MainApp.exe
    └── (app dependencies)
```

---

## 🔧 Version Management

### Launcher Version
- Stored in `Launcher/Launcher.csproj`
- Read from assembly at runtime
- Compared against `launcher-update.xml`

### MainApp Version
- Stored in `MainApp/MainApp.csproj`
- Read by AutoUpdater from app-update.xml
- Can be different from Launcher version

### Example:
```
Current Installation:
├── Launcher.exe v1.0.2
└── MainApp.exe v1.0.5   ← MainApp updated 3 times without touching Launcher

Available Updates:
├── Launcher v1.0.2 (no update)
└── MainApp v1.0.6 (update available!)
```

---

## 🛡️ Safety Features

### 1. **Graceful Degradation**
- If Launcher update fails → Continue to MainApp
- If MainApp update fails → Launch existing MainApp
- If both fail → Launch existing MainApp

### 2. **User Control**
- Launcher update: Can skip (unless mandatory)
- MainApp update: Can skip or remind later
- Both are independent choices

### 3. **Rollback**
- Edit `launcher-update.xml` to point to older version
- Edit `app-update.xml` to point to older version
- Users can "update" back to previous version

---

## 🔍 Comparison: Old vs New

| Feature | Old (Single Channel) | New (Dual Channel) |
|---------|---------------------|-------------------|
| **Update Both** | Download 1 ZIP (164 MB) | Download 2 ZIPs (1 MB each) |
| **Update Launcher** | Download entire package | Download Launcher only (~150 KB) |
| **Update MainApp** | Download entire package | Download MainApp only (~2 MB) |
| **Multiple Apps** | ❌ Not supported | ✅ Fully supported |
| **Reusability** | ❌ Tied together | ✅ Launcher reusable |
| **Flexibility** | ❌ All or nothing | ✅ Independent control |

---

## 💡 Use Cases

### Use Case 1: Bug Fix in Launcher
**Problem:** Found security bug in Launcher
**Solution:** Update only launcher-update.xml → All users get Launcher fix
**Benefit:** No need to re-release all MainApps

### Use Case 2: New Feature in App
**Problem:** Added new feature to ProjectA
**Solution:** Update only app-update.xml for ProjectA
**Benefit:** Other projects unaffected, smaller download

### Use Case 3: Multiple Products
**Problem:** Company has 5 different apps
**Solution:** 1 Launcher + 5 different MainApps
**Benefit:** Maintain one updater for all products

---

## 🎓 Summary

**The new architecture provides:**
- ✅ Independent update channels for Launcher and MainApp
- ✅ Smaller update downloads
- ✅ Reusable Launcher across multiple projects
- ✅ Flexible version management
- ✅ Better user experience
- ✅ Easier maintenance

**Update flow:**
1. Launcher checks itself first
2. If update available → Update & restart
3. Then checks MainApp
4. If update available → Update MainApp
5. Launch MainApp (updated or existing)

🤖 Generated with [Claude Code](https://claude.com/claude-code)

# Reusing Launcher for Multiple Projects

## 🎯 Goal

Use the **same Launcher.exe** for multiple different applications. Just change `config.ini` to point to your app!

---

## 📦 How It Works

```
One Launcher → Many Apps

Launcher.exe (SHARED, SAME FOR ALL)
    ↓
config.ini (DIFFERENT for each app)
    ├── AppUpdateUrl → points to YOUR app's update.xml
    ├── MainAppPath → points to YOUR app's .exe
    └── AppName → YOUR app name
    ↓
Your MainApp (YOUR CUSTOM APPLICATION)
```

---

## ✅ **Example: 3 Different Projects**

### Project A: "My Calculator"
```
MyCalculator/
├── Launcher.exe          ← SAME launcher
├── config.ini            ← Different config
│   └── AppUpdateUrl=.../calculator-update.xml
│   └── MainAppPath=CalculatorApp\Calculator.exe
└── CalculatorApp/
    └── Calculator.exe    ← YOUR calculator app
```

### Project B: "My Text Editor"
```
MyTextEditor/
├── Launcher.exe          ← SAME launcher (copy)
├── config.ini            ← Different config
│   └── AppUpdateUrl=.../texteditor-update.xml
│   └── MainAppPath=EditorApp\TextEditor.exe
└── EditorApp/
    └── TextEditor.exe    ← YOUR text editor app
```

### Project C: "My Game"
```
MyGame/
├── Launcher.exe          ← SAME launcher (copy)
├── config.ini            ← Different config
│   └── AppUpdateUrl=.../game-update.xml
│   └── MainAppPath=GameApp\MyGame.exe
└── GameApp/
    └── MyGame.exe        ← YOUR game
```

**All 3 use the same Launcher.exe!** Only config.ini is different.

---

## 🚀 Quick Start: Use Launcher with Your Project

### Step 1: Copy Launcher Files

From this repository, copy:
```
Auto_Updater/releases/v1.0.2/
├── Launcher.exe
├── *.dll (all DLL files)
└── (copy to your project folder)
```

### Step 2: Create Your App Folder

```
YourProject/
├── Launcher.exe          ← Copied from above
├── *.dll                 ← All dependencies
└── YourApp/              ← Create this folder
    └── YourApp.exe       ← YOUR application here
```

### Step 3: Create config.ini

Create `config.ini` next to Launcher.exe:

```ini
[Updater]
LauncherUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/launcher-update.xml
AppUpdateUrl=https://raw.githubusercontent.com/YOUR_USERNAME/YOUR_PROJECT/master/your-app-update.xml
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

**Key settings to change:**
- `AppUpdateUrl` → Your GitHub repo's update XML
- `MainAppPath` → Path to your app's exe (relative to Launcher)
- `AppName` → Your app's name

### Step 4: Create your-app-update.xml

In your GitHub repository, create `your-app-update.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.0.0</version>
    <url>https://github.com/YOUR_USERNAME/YOUR_PROJECT/releases/download/v1.0.0/YourApp-v1.0.0.zip</url>
    <changelog>https://github.com/YOUR_USERNAME/YOUR_PROJECT/releases/tag/v1.0.0</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">YOUR_APP_ZIP_CHECKSUM</checksum>
</item>
```

### Step 5: Test

1. Run `Launcher.exe`
2. It will check for updates
3. Then launch `YourApp/YourApp.exe`

✅ **Done! Your app now has auto-update!**

---

## 📝 Complete Example: "MyCalculator" App

### 1. Folder Structure

```
MyCalculator/
├── Launcher.exe                    ← From Auto_Updater
├── Autoupdater.NET.Official.dll    ← From Auto_Updater
├── ini-parser.dll                  ← From Auto_Updater
├── (other DLLs)                    ← From Auto_Updater
├── config.ini                      ← Custom for Calculator
└── CalculatorApp/                  ← Your app folder
    ├── Calculator.exe              ← YOUR APPLICATION
    └── (your app dependencies)
```

### 2. config.ini

```ini
[Updater]
LauncherUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/launcher-update.xml
AppUpdateUrl=https://raw.githubusercontent.com/john/MyCalculator/master/calculator-update.xml
MainAppPath=CalculatorApp\Calculator.exe
AutoCheckLauncherUpdate=true
AutoCheckAppUpdate=true
ShowUpdateUI=true
MandatoryLauncherUpdate=false
MandatoryAppUpdate=false

[Application]
AppName=My Calculator
AppVersion=1.0.0
LauncherVersion=1.0.2
```

### 3. calculator-update.xml (in your GitHub repo)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.0.0</version>
    <url>https://github.com/john/MyCalculator/releases/download/v1.0.0/Calculator-v1.0.0.zip</url>
    <changelog>https://github.com/john/MyCalculator/releases/tag/v1.0.0</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">ABC123...</checksum>
</item>
```

### 4. Release Package (Calculator-v1.0.0.zip)

```
Calculator-v1.0.0.zip
└── CalculatorApp/
    ├── Calculator.exe
    └── (dependencies)
```

### 5. When User Runs Launcher

```
1. Launcher.exe starts
2. Reads config.ini
3. Checks launcher-update.xml → Launcher is up to date
4. Checks calculator-update.xml → Calculator is up to date
5. Launches CalculatorApp/Calculator.exe
6. User sees your Calculator app!
```

### 6. When You Release Calculator v1.0.1

```
1. Build new Calculator.exe
2. Create Calculator-v1.0.1.zip
3. Upload to GitHub releases/v1.0.1/
4. Update calculator-update.xml:
   <version>1.0.1.0</version>
   <url>.../Calculator-v1.0.1.zip</url>
5. Push to GitHub
6. Users get update notification automatically!
```

---

## 🔄 Real-World Workflow

### Scenario: You Have 5 Different Apps

```
Your GitHub Repos:
├── Auto_Updater (shared launcher)
├── Calculator
├── TextEditor
├── ImageViewer
├── MusicPlayer
└── GameLauncher

Each app's repo contains:
├── src/ (your app code)
├── app-update.xml
└── README.md

Each app's release contains:
└── AppName-vX.X.X.zip
    └── AppFolder/
        └── App.exe
```

**Distribution:**

```
Calculator Package:
├── Launcher.exe (from Auto_Updater)
├── config.ini → AppUpdateUrl=.../Calculator/.../calculator-update.xml
└── CalculatorApp/

TextEditor Package:
├── Launcher.exe (same file!)
├── config.ini → AppUpdateUrl=.../TextEditor/.../editor-update.xml
└── EditorApp/

... (repeat for each app)
```

**Updates:**

- Update Launcher once → All apps benefit
- Update Calculator → Only Calculator users get update
- Update TextEditor → Only TextEditor users get update
- Each app updates independently!

---

## 🎨 Customization Options

### Option 1: Change App Name

```ini
[Application]
AppName=My Awesome Calculator  ← Shows in update dialogs
```

### Option 2: Disable Launcher Updates

```ini
[Updater]
AutoCheckLauncherUpdate=false  ← Only check your app, not launcher
```

### Option 3: Mandatory App Updates

```ini
[Updater]
MandatoryAppUpdate=true  ← Force users to update
```

### Option 4: Different Update Server

```ini
[Updater]
AppUpdateUrl=https://myserver.com/updates/myapp.xml  ← Your own server
```

### Option 5: Different Folder Structure

```ini
[Updater]
MainAppPath=bin\MyApp.exe      ← App in "bin" folder
# Or
MainAppPath=MyApp.exe          ← App in same folder as Launcher
# Or
MainAppPath=..\MyApp\App.exe   ← App in parent folder
```

---

## 📋 Checklist: Adding Auto-Update to Your Existing App

- [ ] Copy Launcher.exe and DLLs from Auto_Updater
- [ ] Create folder for your app (e.g., YourApp/)
- [ ] Move your app's exe into that folder
- [ ] Create config.ini next to Launcher.exe
- [ ] Set `MainAppPath` to your app's exe path
- [ ] Create your-app-update.xml in your GitHub repo
- [ ] Create GitHub release with your app's ZIP
- [ ] Update your-app-update.xml with version and URL
- [ ] Test: Run Launcher.exe → should launch your app
- [ ] Release v1.0.1 to test auto-update works

---

## 🛠️ Template Files

### Template config.ini

```ini
[Updater]
LauncherUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/launcher-update.xml
AppUpdateUrl=https://raw.githubusercontent.com/GITHUB_USER/PROJECT_NAME/master/app-update.xml
MainAppPath=AppFolder\App.exe
AutoCheckLauncherUpdate=true
AutoCheckAppUpdate=true
ShowUpdateUI=true
MandatoryLauncherUpdate=false
MandatoryAppUpdate=false

[Application]
AppName=My Application
AppVersion=1.0.0
LauncherVersion=1.0.2
```

**Replace:**
- `GITHUB_USER` → Your GitHub username
- `PROJECT_NAME` → Your project name
- `AppFolder\App.exe` → Path to your exe
- `My Application` → Your app name

### Template app-update.xml

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.0.0</version>
    <url>https://github.com/GITHUB_USER/PROJECT_NAME/releases/download/v1.0.0/AppName-v1.0.0.zip</url>
    <changelog>https://github.com/GITHUB_USER/PROJECT_NAME/releases/tag/v1.0.0</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">CALCULATE_WITH_GET-FILEHASH</checksum>
</item>
```

**Replace:**
- `GITHUB_USER` → Your GitHub username
- `PROJECT_NAME` → Your project name
- `AppName` → Your app name
- `CALCULATE_WITH_GET-FILEHASH` → Run: `Get-FileHash YourApp.zip -Algorithm SHA256`

---

## 💡 Tips

### Tip 1: Test Locally First

```
1. Create your folder structure
2. Create config.ini
3. Run Launcher.exe
4. Make sure it launches your app
5. THEN worry about updates
```

### Tip 2: Use Same Version Numbers

```
If your app is v2.5.3:
- In app-update.xml: <version>2.5.3.0</version>
- In config.ini: AppVersion=2.5.3
- Consistency helps!
```

### Tip 3: Keep Launcher Updated

```
Periodically check Auto_Updater repo for new Launcher versions.
Copy new Launcher.exe to all your projects when available.
```

### Tip 4: One Repo Per App

```
✅ GOOD:
├── Calculator (repo)
├── TextEditor (repo)
└── MusicPlayer (repo)

❌ BAD:
└── AllMyApps (one big repo)
```

Each app should have its own repo and update channel.

---

## 🎯 Summary

**What you need:**
1. Launcher.exe + DLLs (from Auto_Updater) → **Reusable for all apps**
2. config.ini → **Different for each app**
3. Your app folder → **Your custom application**
4. app-update.xml in your repo → **Your update manifest**

**To add auto-update to any app:**
1. Copy Launcher files
2. Create config.ini pointing to your app
3. Create app-update.xml in your GitHub repo
4. Done! Your app has auto-update

**Benefits:**
- ✅ One launcher for all your apps
- ✅ Each app updates independently
- ✅ Easy to maintain
- ✅ Small download sizes
- ✅ Professional update system

🤖 Generated with [Claude Code](https://claude.com/claude-code)

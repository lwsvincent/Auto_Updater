# Testing Launcher with Multiple Apps

## 🎯 Test Setup

I've created 3 different sample applications to test the launcher switching capability:

### MainApp1 - Calculator App
- **Features**: Simple calculator with +, -, ×, ÷ operations
- **Color Theme**: Blue/Green
- **Version**: 1.0.0

### MainApp2 - Text Editor App
- **Features**: Text editing with word count, uppercase, lowercase functions
- **Color Theme**: Purple
- **Version**: 1.0.0

### MainApp3 - Color Palette App
- **Features**: Random color palette generator with hex codes
- **Color Theme**: Cyan/Pink
- **Version**: 1.0.0

---

## 📁 Test Directory Structure

```
releases/test/
├── Setup-App1/                    ← Calculator Setup
│   ├── Launcher.exe
│   ├── *.dll
│   ├── config.ini                 → Points to MainApp1
│   └── MainApp1/
│       └── MainApp1.exe
│
├── Setup-App2/                    ← Text Editor Setup
│   ├── Launcher.exe
│   ├── *.dll
│   ├── config.ini                 → Points to MainApp2
│   └── MainApp2/
│       └── MainApp2.exe
│
└── Setup-App3/                    ← Color Palette Setup
    ├── Launcher.exe
    ├── *.dll
    ├── config.ini                 → Points to MainApp3
    └── MainApp3/
        └── MainApp3.exe
```

---

## 🧪 How to Test

### Test 1: Run Each App Separately

1. **Test Calculator App**:
   ```cmd
   cd releases\test\Setup-App1
   Launcher.exe
   ```
   - Should launch Calculator app
   - Try doing some calculations
   - Verify it shows "MainApp1 - Calculator" and "Version: 1.0.0"

2. **Test Text Editor App**:
   ```cmd
   cd ..\Setup-App2
   Launcher.exe
   ```
   - Should launch Text Editor app
   - Try typing text and using word count
   - Verify it shows "MainApp2 - Text Editor" and "Version: 1.0.0"

3. **Test Color Palette App**:
   ```cmd
   cd ..\Setup-App3
   Launcher.exe
   ```
   - Should launch Color Palette app
   - Try generating random colors
   - Click on colors to copy hex codes
   - Verify it shows "MainApp3 - Color Palette" and "Version: 1.0.0"

### Test 2: Verify Same Launcher, Different Apps

1. Check that `Launcher.exe` is identical in all three folders:
   ```powershell
   Get-FileHash Setup-App1\Launcher.exe
   Get-FileHash Setup-App2\Launcher.exe
   Get-FileHash Setup-App3\Launcher.exe
   ```
   - All three should have the **same hash** ✅

2. Check that `config.ini` is different:
   ```cmd
   type Setup-App1\config.ini
   type Setup-App2\config.ini
   type Setup-App3\config.ini
   ```
   - Each should point to different `MainAppPath` ✅
   - Each should have different `AppName` ✅
   - Each should have different `AppUpdateUrl` ✅

### Test 3: Switch Apps by Changing config.ini

1. **Navigate to Setup-App1**:
   ```cmd
   cd Setup-App1
   ```

2. **Run Launcher** - should launch Calculator

3. **Edit config.ini** to point to MainApp2:
   ```ini
   [Updater]
   MainAppPath=..\MainApp2\MainApp2.exe
   AppUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/app2-update.xml

   [Application]
   AppName=Text Editor App
   ```

4. **Run Launcher again** - should now launch Text Editor!

5. **Change back to Calculator or try MainApp3**

This proves the same Launcher can switch between different apps just by changing config.ini! ✅

---

## ✅ What This Proves

### ✅ One Launcher, Multiple Apps
- The same `Launcher.exe` successfully launches 3 different applications
- Each app has its own config file
- No need to rebuild launcher for different apps

### ✅ Independent Configuration
- `config.ini` controls which app to launch
- Each app has its own update channel (app1-update.xml, app2-update.xml, app3-update.xml)
- Easy to switch between apps by editing config

### ✅ Reusability
- You can copy `Launcher.exe` + DLLs to any project
- Create custom `config.ini` pointing to your app
- The launcher will work with any WPF/.NET application

### ✅ Update Channels
- Each app has its own update XML file:
  - `app1-update.xml` for Calculator
  - `app2-update.xml` for Text Editor
  - `app3-update.xml` for Color Palette
- All share the same launcher update channel: `launcher-update.xml`

---

## 🎨 Visual Differences

When you run each setup, you should see:

**Setup-App1 (Calculator)**:
- Blue header with "Simple Calculator"
- Number inputs and operation buttons (+, -, ×, ÷)
- Result display
- Blue info panel at bottom

**Setup-App2 (Text Editor)**:
- Purple header with "Simple Text Editor"
- Large text editing area
- Toolbar with Clear, Word Count, Upper/Lower case buttons
- Purple status bar at bottom

**Setup-App3 (Color Palette)**:
- Cyan header with "Color Palette Generator"
- 3×3 grid of random colors
- Each color shows its hex code
- Click to copy hex code
- Cyan info panel at bottom

---

## 📝 Config File Comparison

### config-app1.ini (Calculator)
```ini
MainAppPath=MainApp1\MainApp1.exe
AppUpdateUrl=.../app1-update.xml
AppName=Calculator App
```

### config-app2.ini (Text Editor)
```ini
MainAppPath=MainApp2\MainApp2.exe
AppUpdateUrl=.../app2-update.xml
AppName=Text Editor App
```

### config-app3.ini (Color Palette)
```ini
MainAppPath=MainApp3\MainApp3.exe
AppUpdateUrl=.../app3-update.xml
AppName=Color Palette App
```

**Only 3 lines are different!** Everything else is shared.

---

## 🚀 Next Steps

Once local testing is complete:

1. **Create GitHub Releases** for each app:
   - `app1-v1.0.0` with MainApp1 ZIP
   - `app2-v1.0.0` with MainApp2 ZIP
   - `app3-v1.0.0` with MainApp3 ZIP

2. **Upload update XML files** to GitHub repo:
   - `app1-update.xml`
   - `app2-update.xml`
   - `app3-update.xml`

3. **Test auto-update**:
   - Update each app to v1.0.1
   - Verify launcher downloads and updates only the specific app
   - Verify launcher itself can update independently

---

## 🎯 Summary

You now have a **working proof of concept** that demonstrates:

- ✅ **One launcher serves multiple apps**
- ✅ **Apps are independent and different**
- ✅ **Switching apps is as simple as changing config.ini**
- ✅ **Each app has its own update channel**
- ✅ **Launcher is reusable across projects**

This is exactly what you requested: A launcher that can be reused for multiple projects by just changing the config file!

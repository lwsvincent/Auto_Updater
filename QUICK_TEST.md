# Quick Test Instructions

## 🎯 Test the Launcher Switching

### Test 1: Launch Calculator App
```cmd
cd releases\test\Setup-App1
Launcher.exe
```
**Expected**: Blue calculator app with +, -, ×, ÷ buttons

---

### Test 2: Launch Text Editor App
```cmd
cd ..\Setup-App2
Launcher.exe
```
**Expected**: Purple text editor with word count features

---

### Test 3: Launch Color Palette App
```cmd
cd ..\Setup-App3
Launcher.exe
```
**Expected**: Cyan color palette generator

---

## ✅ Verify Same Launcher

Check all three launchers are identical:

```powershell
Get-FileHash releases\test\Setup-App1\Launcher.exe
Get-FileHash releases\test\Setup-App2\Launcher.exe
Get-FileHash releases\test\Setup-App3\Launcher.exe
```

**All three should have the SAME hash** - proving it's the same launcher!

---

## 🔄 Test Switching Apps

1. Go to Setup-App1:
   ```cmd
   cd releases\test\Setup-App1
   Launcher.exe
   ```
   → Should launch **Calculator**

2. Close the Calculator

3. Edit `config.ini` and change `MainAppPath`:
   ```ini
   MainAppPath=..\Setup-App2\MainApp2\MainApp2.exe
   ```

4. Run Launcher again:
   ```cmd
   Launcher.exe
   ```
   → Should now launch **Text Editor** instead!

This proves: **Same launcher, different app, just by changing config!** ✅

---

## 📦 What You Have

- **3 different applications** (Calculator, Text Editor, Color Palette)
- **1 shared launcher** (same Launcher.exe in all 3 folders)
- **3 different configs** (each pointing to different app)
- **3 different update channels** (app1-update.xml, app2-update.xml, app3-update.xml)

Ready to test! See `TEST_GUIDE.md` for detailed testing instructions.

# Auto_Updater

A C# WPF application demonstrating automatic updates using AutoUpdater.NET with GitHub Releases.

## Features

- ✅ Automatic update checking
- ✅ GitHub Releases integration
- ✅ Manual update URL input
- ✅ Checksum verification for secure downloads
- ✅ User-friendly update UI
- ✅ Support for both public and private repositories

## Tech Stack

- **Framework**: .NET (WPF)
- **Library**: [AutoUpdater.NET](https://github.com/ravibpatel/AutoUpdater.NET)
- **Update Hosting**: GitHub Releases
- **Language**: C#

## Project Structure

```
Auto_Updater/
├── Auto_Updater/           # Main WPF application
│   ├── MainWindow.xaml     # UI layout
│   ├── MainWindow.xaml.cs  # Update logic
│   └── Auto_Updater.csproj # Project file
├── update.xml              # Update manifest (host on GitHub)
└── README.md               # This file
```

## Setup Instructions

### 1. Configure Update URL

Edit `MainWindow.xaml.cs` and replace `YOUR_USERNAME` with your GitHub username:

```csharp
private const string UpdateUrl = "https://raw.githubusercontent.com/YOUR_USERNAME/Auto_Updater/main/update.xml";
```

### 2. Update the XML Manifest

Edit `update.xml` and update:
- `YOUR_USERNAME` with your GitHub username
- `PUT_CHECKSUM_HERE` with the SHA256 hash of your installer

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.0.0</version>
    <url>https://github.com/YOUR_USERNAME/Auto_Updater/releases/download/v1.0.0/Auto_Updater-Setup.exe</url>
    <changelog>https://github.com/YOUR_USERNAME/Auto_Updater/releases/tag/v1.0.0</changelog>
    <mandatory>false</mandatory>
    <checksum algorithm="SHA256">PUT_CHECKSUM_HERE</checksum>
</item>
```

### 3. Build the Application

```bash
cd Auto_Updater
dotnet build -c Release
```

### 4. Create an Installer

Create an installer (using Inno Setup, WiX, or any installer creator) from the published files:

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

### 5. Calculate Checksum

```bash
# Windows PowerShell
Get-FileHash Auto_Updater-Setup.exe -Algorithm SHA256

# Linux/Mac
shasum -a 256 Auto_Updater-Setup.exe
```

### 6. Create GitHub Release

1. Go to your repository on GitHub
2. Click "Releases" → "Create a new release"
3. Tag version: `v1.0.0`
4. Upload your installer file: `Auto_Updater-Setup.exe`
5. Publish release

### 7. Update XML with Real Values

Update `update.xml` with:
- Real checksum from step 5
- Real GitHub URL from step 6
- Commit and push to GitHub

## How It Works

1. **Check for Updates**: Application calls `AutoUpdater.Start(UpdateUrl)`
2. **Download XML**: AutoUpdater.NET downloads `update.xml` from GitHub
3. **Compare Versions**: Compares current version with XML version
4. **Download Installer**: If newer version exists, downloads installer from GitHub Release
5. **Verify Integrity**: Validates checksum before installation
6. **Install Update**: Prompts user to install the update

## For Private Repositories

If using a private repository, uncomment and configure authentication in `MainWindow.xaml.cs`:

```csharp
// Generate a Personal Access Token (PAT) from GitHub
AutoUpdater.BasicAuthXML = new BasicAuthentication("username", "PAT_TOKEN");
```

Or better yet, proxy through your own server to avoid embedding tokens.

## Update Manifest Fields

| Field | Description |
|-------|-------------|
| `version` | Version number (e.g., 1.0.0.0) |
| `url` | Direct download URL to installer |
| `changelog` | URL to release notes |
| `mandatory` | `true` = force update, `false` = optional |
| `checksum` | SHA256 hash for integrity verification |

## Testing Updates

1. Build version 1.0.0 and create release
2. Update version in `.csproj` to 1.1.0
3. Build new version and create new release
4. Update `update.xml` with v1.1.0 info
5. Run v1.0.0 app and click "Check for Updates"

## Security Notes

- Always use HTTPS URLs
- Always include checksums for integrity verification
- For private repos, avoid embedding PAT tokens directly in code
- Consider using a backend proxy for authentication

## License

Free to use for commercial and personal projects.

## Resources

- [AutoUpdater.NET Documentation](https://github.com/ravibpatel/AutoUpdater.NET)
- [GitHub Releases Documentation](https://docs.github.com/en/repositories/releasing-projects-on-github)

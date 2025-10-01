using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AutoUpdaterDotNET;
using IniParser;
using IniParser.Model;

namespace Launcher;

public partial class MainWindow : Window
{
    private readonly string configPath = "config.ini";
    private IniData? config;
    private string? updateUrl;
    private string? mainAppPath;
    private bool autoCheckUpdate;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        StartLoadingAnimation();

        await Task.Delay(500); // Brief delay for UI to show

        try
        {
            UpdateStatus("Loading configuration...", "Reading config.ini file");
            if (!LoadConfiguration())
            {
                ShowErrorAndExit("Configuration file (config.ini) not found or invalid.");
                return;
            }

            if (autoCheckUpdate)
            {
                UpdateStatus("Checking for updates...", "Connecting to update server");
                await Task.Delay(500);

                CheckForUpdates();
            }
            else
            {
                UpdateStatus("Skipping update check...", "Auto-update is disabled");
                await Task.Delay(1000);
                LaunchMainApp();
            }
        }
        catch (Exception ex)
        {
            ShowErrorAndExit($"Error: {ex.Message}");
        }
    }

    private bool LoadConfiguration()
    {
        try
        {
            if (!File.Exists(configPath))
            {
                // Create default config if it doesn't exist
                CreateDefaultConfig();
            }

            var parser = new FileIniDataParser();
            config = parser.ReadFile(configPath);

            updateUrl = config["Updater"]["UpdateUrl"];
            mainAppPath = config["Updater"]["MainAppPath"];
            autoCheckUpdate = bool.Parse(config["Updater"]["AutoCheckUpdate"] ?? "true");

            return !string.IsNullOrWhiteSpace(updateUrl) && !string.IsNullOrWhiteSpace(mainAppPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load configuration: {ex.Message}", "Configuration Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private void CreateDefaultConfig()
    {
        var defaultConfig = @"[Updater]
UpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/update.xml
MainAppPath=MainApp\MainApp.exe
AutoCheckUpdate=true
ShowUpdateUI=true
MandatoryUpdate=false

[Application]
AppName=Auto Updater Demo
Version=1.0.2";

        File.WriteAllText(configPath, defaultConfig);
    }

    private void CheckForUpdates()
    {
        try
        {
            // Configure AutoUpdater
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.Mandatory = config?["Updater"]["MandatoryUpdate"] == "true";
            AutoUpdater.ReportErrors = true;

            // Subscribe to update check completed event
            AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;

            // Subscribe to application exit event (after update download)
            AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;

            // Start update check
            AutoUpdater.Start(updateUrl);
        }
        catch (Exception ex)
        {
            UpdateStatus("Update check failed", ex.Message);
            Task.Delay(2000).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
        }
    }

    private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs? args)
    {
        if (args == null)
        {
            UpdateStatus("Update check failed", "No update information available");
            Task.Delay(2000).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
            return;
        }

        if (args.Error != null)
        {
            UpdateStatus("Update check failed", args.Error.Message);
            Task.Delay(2000).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
            return;
        }

        if (args.IsUpdateAvailable)
        {
            UpdateStatus("Update available!", $"Version {args.CurrentVersion} is available");
            // AutoUpdater will show its own dialog
            // If user skips or closes, we launch the main app
        }
        else
        {
            UpdateStatus("No updates available", "Application is up to date");
            Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
        }
    }

    private void AutoUpdater_ApplicationExitEvent()
    {
        // This is called when the update is downloaded and ready to install
        // The application will close and the installer will run
        Dispatcher.Invoke(() =>
        {
            UpdateStatus("Installing update...", "Application will restart after installation");
        });
    }

    private void LaunchMainApp()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mainAppPath))
            {
                ShowErrorAndExit("Main application path not configured.");
                return;
            }

            UpdateStatus("Launching application...", $"Starting {Path.GetFileName(mainAppPath)}");

            var fullPath = Path.GetFullPath(mainAppPath);
            if (!File.Exists(fullPath))
            {
                ShowErrorAndExit($"Main application not found: {fullPath}");
                return;
            }

            // Launch the main application
            Process.Start(new ProcessStartInfo
            {
                FileName = fullPath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(fullPath) ?? Environment.CurrentDirectory
            });

            // Close the launcher
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            ShowErrorAndExit($"Failed to launch main application: {ex.Message}");
        }
    }

    private void UpdateStatus(string status, string detail)
    {
        Dispatcher.Invoke(() =>
        {
            StatusText.Text = status;
            DetailText.Text = detail;
        });
    }

    private void ShowErrorAndExit(string message)
    {
        Dispatcher.Invoke(() =>
        {
            StopLoadingAnimation();
            MessageBox.Show(message, "Launcher Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        });
    }

    private void StartLoadingAnimation()
    {
        var animation = new DoubleAnimation
        {
            From = 0,
            To = 360,
            Duration = TimeSpan.FromSeconds(2),
            RepeatBehavior = RepeatBehavior.Forever
        };

        LoadingRotation.BeginAnimation(RotateTransform.AngleProperty, animation);
    }

    private void StopLoadingAnimation()
    {
        LoadingRotation.BeginAnimation(RotateTransform.AngleProperty, null);
    }
}

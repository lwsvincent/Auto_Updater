using System.Diagnostics;
using System.IO;
using System.Reflection;
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

    // Configuration values
    private string? launcherUpdateUrl;
    private string? appUpdateUrl;
    private string? mainAppPath;
    private bool autoCheckLauncherUpdate;
    private bool autoCheckAppUpdate;
    private bool launcherUpdateChecked = false;
    private bool appUpdateChecked = false;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        StartLoadingAnimation();
        await Task.Delay(500);

        try
        {
            UpdateStatus("Loading configuration...", "Reading config.ini file");
            if (!LoadConfiguration())
            {
                ShowErrorAndExit("Configuration file (config.ini) not found or invalid.");
                return;
            }

            // STEP 1: Check for launcher updates first
            if (autoCheckLauncherUpdate)
            {
                UpdateStatus("Checking launcher updates...", "Checking for updater updates");
                await Task.Delay(500);
                CheckLauncherUpdate();
            }
            else
            {
                launcherUpdateChecked = true;
                ProceedToAppUpdate();
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
                CreateDefaultConfig();
            }

            var parser = new FileIniDataParser();
            parser.Parser.Configuration.CommentString = "#";
            parser.Parser.Configuration.AllowCreateSectionsOnFly = true;

            config = parser.ReadFile(configPath);

            launcherUpdateUrl = config["Updater"]["LauncherUpdateUrl"];
            appUpdateUrl = config["Updater"]["AppUpdateUrl"];
            mainAppPath = config["Updater"]["MainAppPath"];
            autoCheckLauncherUpdate = bool.Parse(config["Updater"]["AutoCheckLauncherUpdate"] ?? "true");
            autoCheckAppUpdate = bool.Parse(config["Updater"]["AutoCheckAppUpdate"] ?? "true");

            return !string.IsNullOrWhiteSpace(mainAppPath);
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
LauncherUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/launcher-update.xml
AppUpdateUrl=https://raw.githubusercontent.com/lwsvincent/Auto_Updater/master/app-update.xml
MainAppPath=MainApp\MainApp.exe
AutoCheckLauncherUpdate=true
AutoCheckAppUpdate=true
ShowUpdateUI=true
MandatoryLauncherUpdate=false
MandatoryAppUpdate=false

[Application]
AppName=Auto Updater Demo
AppVersion=1.0.2
LauncherVersion=1.0.2";

        File.WriteAllText(configPath, defaultConfig);
    }

    private void CheckLauncherUpdate()
    {
        try
        {
            // Unsubscribe previous events
            AutoUpdater.CheckForUpdateEvent -= LauncherUpdate_CheckForUpdateEvent;
            AutoUpdater.ApplicationExitEvent -= LauncherUpdate_ApplicationExitEvent;

            // Subscribe to launcher update events
            AutoUpdater.CheckForUpdateEvent += LauncherUpdate_CheckForUpdateEvent;
            AutoUpdater.ApplicationExitEvent += LauncherUpdate_ApplicationExitEvent;

            // Configure AutoUpdater for launcher
            AutoUpdater.ShowSkipButton = true;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.Mandatory = config?["Updater"]["MandatoryLauncherUpdate"] == "true";
            AutoUpdater.ReportErrors = false; // Don't show errors for launcher update

            // Get current launcher version
            AutoUpdater.InstalledVersion = Assembly.GetExecutingAssembly().GetName().Version;
            AutoUpdater.AppTitle = "Launcher Update";

            // Start launcher update check
            AutoUpdater.Start(launcherUpdateUrl);
        }
        catch (Exception ex)
        {
            UpdateStatus("Launcher update check failed", ex.Message);
            launcherUpdateChecked = true;
            ProceedToAppUpdate();
        }
    }

    private void LauncherUpdate_CheckForUpdateEvent(UpdateInfoEventArgs? args)
    {
        if (args == null || args.Error != null || !args.IsUpdateAvailable)
        {
            // No launcher update available, proceed to app update
            launcherUpdateChecked = true;
            Dispatcher.Invoke(ProceedToAppUpdate);
            return;
        }

        UpdateStatus("Launcher update available!", $"New launcher version {args.CurrentVersion} is available");
        // AutoUpdater will show dialog
        // If user installs, ApplicationExitEvent will be called
        // If user skips, we'll proceed to app update
    }

    private void LauncherUpdate_ApplicationExitEvent()
    {
        // Launcher is updating itself - the app will close and restart
        Dispatcher.Invoke(() =>
        {
            UpdateStatus("Updating launcher...", "Launcher will restart after update");
        });
    }

    private void ProceedToAppUpdate()
    {
        if (!launcherUpdateChecked)
            return;

        // STEP 2: Check for MainApp updates
        if (autoCheckAppUpdate)
        {
            UpdateStatus("Checking app updates...", "Checking for application updates");
            Task.Delay(500).ContinueWith(_ => Dispatcher.Invoke(CheckAppUpdate));
        }
        else
        {
            appUpdateChecked = true;
            LaunchMainApp();
        }
    }

    private void CheckAppUpdate()
    {
        try
        {
            // Unsubscribe previous events
            AutoUpdater.CheckForUpdateEvent -= LauncherUpdate_CheckForUpdateEvent;
            AutoUpdater.ApplicationExitEvent -= LauncherUpdate_ApplicationExitEvent;
            AutoUpdater.CheckForUpdateEvent -= AppUpdate_CheckForUpdateEvent;

            // Subscribe to app update events
            AutoUpdater.CheckForUpdateEvent += AppUpdate_CheckForUpdateEvent;

            // Configure AutoUpdater for app
            AutoUpdater.ShowSkipButton = true;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.Mandatory = config?["Updater"]["MandatoryAppUpdate"] == "true";
            AutoUpdater.ReportErrors = true;
            AutoUpdater.AppTitle = "Application Update";

            // Reset installed version (app version might be different from launcher)
            AutoUpdater.InstalledVersion = null;

            // Start app update check
            AutoUpdater.Start(appUpdateUrl);
        }
        catch (Exception ex)
        {
            UpdateStatus("App update check failed", ex.Message);
            appUpdateChecked = true;
            LaunchMainApp();
        }
    }

    private void AppUpdate_CheckForUpdateEvent(UpdateInfoEventArgs? args)
    {
        if (args == null)
        {
            appUpdateChecked = true;
            Dispatcher.Invoke(LaunchMainApp);
            return;
        }

        if (args.Error != null)
        {
            UpdateStatus("App update check failed", args.Error.Message);
            appUpdateChecked = true;
            Task.Delay(2000).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
            return;
        }

        if (args.IsUpdateAvailable)
        {
            UpdateStatus("App update available!", $"Version {args.CurrentVersion} is available");
            // AutoUpdater will show dialog
            // User can skip or update
            // After dialog closes (skip/update/error), we launch the app
            appUpdateChecked = true;
            Task.Delay(1000).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
        }
        else
        {
            UpdateStatus("App is up to date", "No updates available");
            appUpdateChecked = true;
            Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(LaunchMainApp));
        }
    }

    private void LaunchMainApp()
    {
        if (!appUpdateChecked)
            return;

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

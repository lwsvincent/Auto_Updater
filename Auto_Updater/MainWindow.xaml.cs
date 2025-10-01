using System.Windows;
using AutoUpdaterDotNET;

namespace Auto_Updater;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Update XML URL - Replace this with your GitHub release XML URL
    private const string UpdateUrl = "https://raw.githubusercontent.com/YOUR_USERNAME/Auto_Updater/main/update.xml";

    public MainWindow()
    {
        InitializeComponent();

        // Optional: Check for updates on startup
        // CheckForUpdates();
    }

    private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        CheckForUpdates();
    }

    private void ManualUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        // Allow user to input custom update URL
        var inputDialog = new InputDialog("Enter Update XML URL:", UpdateUrl);
        if (inputDialog.ShowDialog() == true)
        {
            string customUrl = inputDialog.InputText;
            if (!string.IsNullOrWhiteSpace(customUrl))
            {
                CheckForUpdates(customUrl);
            }
        }
    }

    private void CheckForUpdates(string? customUrl = null)
    {
        try
        {
            StatusText.Text = "Status: Checking for updates...";

            // Configure AutoUpdater
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.Mandatory = false;
            AutoUpdater.ReportErrors = true;

            // Optional: Add custom headers for authentication
            // AutoUpdater.HttpUserAgent = "AutoUpdater";

            // Optional: For GitHub private releases with PAT
            // AutoUpdater.BasicAuthXML = new BasicAuthentication("username", "PAT_TOKEN");

            // Start the update check
            string urlToCheck = customUrl ?? UpdateUrl;
            AutoUpdater.Start(urlToCheck);

            StatusText.Text = "Status: Update check completed";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Status: Error - {ex.Message}";
            MessageBox.Show($"Error checking for updates: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

// Simple input dialog for manual URL entry
public partial class InputDialog : Window
{
    public string InputText { get; private set; } = string.Empty;

    private System.Windows.Controls.TextBox? textBox;

    public InputDialog(string prompt, string defaultValue)
    {
        Title = "Input";
        Width = 500;
        Height = 150;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var grid = new System.Windows.Controls.Grid();
        grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = GridLength.Auto });

        var stackPanel = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };
        stackPanel.Children.Add(new System.Windows.Controls.TextBlock { Text = prompt, Margin = new Thickness(0, 0, 0, 10) });

        textBox = new System.Windows.Controls.TextBox { Text = defaultValue };
        stackPanel.Children.Add(textBox);

        System.Windows.Controls.Grid.SetRow(stackPanel, 0);
        grid.Children.Add(stackPanel);

        var buttonPanel = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10)
        };

        var okButton = new System.Windows.Controls.Button
        {
            Content = "OK",
            Width = 80,
            Height = 30,
            Margin = new Thickness(0, 0, 10, 0)
        };
        okButton.Click += (s, e) => { InputText = textBox.Text; DialogResult = true; Close(); };

        var cancelButton = new System.Windows.Controls.Button
        {
            Content = "Cancel",
            Width = 80,
            Height = 30
        };
        cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);

        System.Windows.Controls.Grid.SetRow(buttonPanel, 1);
        grid.Children.Add(buttonPanel);

        Content = grid;
    }
}
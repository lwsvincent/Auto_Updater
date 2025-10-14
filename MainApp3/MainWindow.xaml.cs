using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace MainApp3
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            GenerateColors();
        }

        private void GenerateColors_Click(object sender, RoutedEventArgs e)
        {
            GenerateColors();
        }

        private void GenerateColors()
        {
            colorGrid.Children.Clear();

            for (int i = 0; i < 9; i++)
            {
                var color = GenerateRandomColor();
                var border = CreateColorBorder(color);
                colorGrid.Children.Add(border);
            }
        }

        private Color GenerateRandomColor()
        {
            return Color.FromRgb(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256)
            );
        }

        private Border CreateColorBorder(Color color)
        {
            var hexCode = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

            var textBlock = new TextBlock
            {
                Text = hexCode,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = GetContrastColor(color),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var border = new Border
            {
                Background = new SolidColorBrush(color),
                Child = textBlock,
                Margin = new Thickness(5),
                Cursor = Cursors.Hand,
                Tag = hexCode
            };

            border.MouseLeftButtonDown += ColorBorder_Click;

            return border;
        }

        private Brush GetContrastColor(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Brushes.Black : Brushes.White;
        }

        private void ColorBorder_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string hexCode)
            {
                Clipboard.SetText(hexCode);
                MessageBox.Show($"Color {hexCode} copied to clipboard!",
                    "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

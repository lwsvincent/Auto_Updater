using System.Windows;

namespace MainApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.Clear();
            txtStatus.Text = "Text cleared";
        }

        private void WordCount_Click(object sender, RoutedEventArgs e)
        {
            string text = txtEditor.Text;
            int wordCount = string.IsNullOrWhiteSpace(text) ? 0 :
                text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int charCount = text.Length;

            MessageBox.Show($"Words: {wordCount}\nCharacters: {charCount}",
                "Word Count", MessageBoxButton.OK, MessageBoxImage.Information);
            txtStatus.Text = $"Words: {wordCount}, Chars: {charCount}";
        }

        private void UpperCase_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = txtEditor.Text.ToUpper();
            txtStatus.Text = "Converted to uppercase";
        }

        private void LowerCase_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = txtEditor.Text.ToLower();
            txtStatus.Text = "Converted to lowercase";
        }
    }
}

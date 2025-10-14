using System.Windows;

namespace MainApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Calculate((a, b) => a + b);
        }

        private void Subtract_Click(object sender, RoutedEventArgs e)
        {
            Calculate((a, b) => a - b);
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            Calculate((a, b) => a * b);
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            Calculate((a, b) => b != 0 ? a / b : double.NaN);
        }

        private void Calculate(Func<double, double, double> operation)
        {
            if (double.TryParse(txtNumber1.Text, out double num1) &&
                double.TryParse(txtNumber2.Text, out double num2))
            {
                double result = operation(num1, num2);
                txtResult.Text = double.IsNaN(result) ? "Cannot divide by zero" : result.ToString();
            }
            else
            {
                txtResult.Text = "Invalid input";
            }
        }
    }
}

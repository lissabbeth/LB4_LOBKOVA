using System.Windows;

namespace LB4_LOBKOVA
{
    public partial class DialogWindow : Window
    {
        public double LowerLimit { get; private set; }
        public double UpperLimit { get; private set; }
        public int NumDivisions { get; private set; }

        public DialogWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LowerLimit = double.Parse(LowerLimitInput.Text);
                UpperLimit = double.Parse(UpperLimitInput.Text);
                NumDivisions = int.Parse(NumDivisionsInput.Text);
                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Please enter valid numbers.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

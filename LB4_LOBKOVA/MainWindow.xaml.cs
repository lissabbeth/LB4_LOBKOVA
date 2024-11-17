using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LB4_LOBKOVA
{
    public partial class MainWindow : Window
    {
        private double lowerLimit;
        private double upperLimit;
        private int divisions;

        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();

            // Настройка BackgroundWorker
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void InputParametersButton_Click(object sender, RoutedEventArgs e)
        {
            DialogWindow dialog = new DialogWindow();
            if (dialog.ShowDialog() == true)
            {
                lowerLimit = dialog.LowerLimit;
                upperLimit = dialog.UpperLimit;
                divisions = dialog.NumDivisions;

                DispatcherButton.IsEnabled = true;
                BackgroundWorkerButton.IsEnabled = true;

                MessageBox.Show($"Parameters set:\nLower Limit: {lowerLimit}\nUpper Limit: {upperLimit}\nDivisions: {divisions}");
            }
        }

        private void DispatcherCalculate_Click(object sender, RoutedEventArgs e)
        {
            ToggleButtons(false); // Отключение кнопок
            ProgressBar.Value = 0;
            ProgressText.Text = "0%";

            Thread calculationThread = new Thread(() =>
            {
                double result = CalculateIntegral(lowerLimit, upperLimit, divisions);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show($"Result (Dispatcher): {result}");
                    ToggleButtons(true); // Включение кнопок
                }));
            });

            calculationThread.Start();
        }

        private void BackgroundWorkerCalculate_Click(object sender, RoutedEventArgs e)
        {
            ToggleButtons(false); // Отключение кнопок
            ProgressBar.Value = 0;
            ProgressText.Text = "0%";

            backgroundWorker.RunWorkerAsync(new { A = lowerLimit, B = upperLimit, N = divisions });
        }

        private double CalculateIntegral(double a, double b, int n)
        {
            double h = (b - a) / n; // Шаг разбиения
            double result = 0;
            int step = Math.Max(1, n / 100); // Шаг обновления прогресса (обеспечивает шаг >= 1)

            for (int i = 1; i <= n; i++)
            {
                double x = a + h * i;
                result += Math.Pow(x, 3); // Здесь f(x) = x^3

                if (i % step == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ProgressBar.Value = (double)i / n * 100;
                        ProgressText.Text = $"{(int)((double)i / n * 100)}%";
                    }));
                }
            }

            return result * h;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            dynamic data = e.Argument;
            double a = data.A;
            double b = data.B;
            int n = data.N;

            double h = (b - a) / n;
            double result = 0;

            for (int i = 1; i <= n; i++)
            {
                double x = a + h * i;
                result += Math.Pow(x, 3);

                if (i % (n / 100) == 0)
                {
                    (sender as BackgroundWorker)?.ReportProgress((int)((double)i / n * 100));
                }
            }

            e.Result = result * h;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            ProgressText.Text = $"{e.ProgressPercentage}%";
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show($"Error: {e.Error.Message}");
            }
            else
            {
                MessageBox.Show($"Result (BackgroundWorker): {e.Result}");
            }

            ToggleButtons(true); // Включение кнопок
        }

        private void ToggleButtons(bool isEnabled)
        {
            Dispatcher.Invoke(() =>
            {
                DispatcherButton.IsEnabled = isEnabled;
                BackgroundWorkerButton.IsEnabled = isEnabled;
            });
        }
    }
}

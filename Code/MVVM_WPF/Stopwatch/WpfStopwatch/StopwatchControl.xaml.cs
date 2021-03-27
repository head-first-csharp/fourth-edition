using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfStopwatch
{
    using Stopwatch.ViewModel;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for StopwatchControl.xaml
    /// </summary>
    public partial class StopwatchControl : UserControl
    {
        DispatcherTimer _timer = new DispatcherTimer();
        StopwatchViewModel _stopwatchViewModel;

        public StopwatchControl()
        {
            InitializeComponent();

            _stopwatchViewModel = Resources["viewModel"] as StopwatchViewModel;

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _stopwatchViewModel.OnPropertyChanged(String.Empty);
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            _stopwatchViewModel.StartStop();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _stopwatchViewModel.Reset();
        }

        private void LapButton_Click(object sender, RoutedEventArgs e)
        {
            _stopwatchViewModel.LapTime();
        }
    }
}

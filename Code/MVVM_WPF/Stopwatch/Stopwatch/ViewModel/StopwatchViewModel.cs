using System;
using System.Collections.Generic;
using System.Text;

namespace Stopwatch.ViewModel
{
    using Model;
    using System.ComponentModel;

    public class StopwatchViewModel : INotifyPropertyChanged
    {
        private readonly StopwatchModel _model = new StopwatchModel();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void StartStop() => _model.Running = !_model.Running;

        public void Reset() => _model.Reset();

        public string Hours => _model.Elapsed.Hours.ToString("D2");

        public string Minutes => _model.Elapsed.Minutes.ToString("D2");

        public string Seconds => _model.Elapsed.Seconds.ToString("D2");

        public object Tenths => ((int)(_model.Elapsed.Milliseconds / 100M)).ToString();

        public void LapTime() => _model.SetLapTime();

        public string LapHours => _model.LapTime.Hours.ToString("D2");

        public string LapMinutes => _model.LapTime.Minutes.ToString("D2");

        public string LapSeconds => _model.LapTime.Seconds.ToString("D2");

        public string LapTenths => ((int)(_model.LapTime.Milliseconds / 100M)).ToString();
    }
}

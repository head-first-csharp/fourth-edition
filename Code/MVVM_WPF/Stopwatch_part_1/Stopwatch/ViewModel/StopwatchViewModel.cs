using System;
using System.Collections.Generic;
using System.Text;

namespace Stopwatch.ViewModel
{
    using Model;

    class StopwatchViewModel
    {
        private readonly StopwatchModel _model = new StopwatchModel();

        public void StartStop() => _model.Running = true;

        public void Reset() => _model.Reset();

        public string Hours => _model.Elapsed.Hours.ToString("D2");

        public string Minutes => _model.Elapsed.Minutes.ToString("D2");

        public string Seconds => _model.Elapsed.Seconds.ToString("D2");

        public object Tenths => ((int)(_model.Elapsed.Milliseconds / 100M)).ToString();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Stopwatch.Model
{
    class StopwatchModel
    {
        private DateTime _startedTime;

        /// <summary>
        /// The constructor resets the stopwatch
        /// </summary>
        public StopwatchModel() => Reset();

        /// <summary>
        /// Returns true if the stopwatch is running
        /// </summary>
        public bool Running
        {
            get => _startedTime != DateTime.MinValue;
            set
            {
                if (value && !Running) _startedTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Returns the elapsed time, or zero if the stopwatch is not running
        /// </summary>
        public TimeSpan Elapsed => Running ? DateTime.Now - _startedTime : TimeSpan.Zero;

        /// <summary>
        /// Resets the stopwatch by setting its started time to DateTime.MinValue
        /// </summary>
        public void Reset() => _startedTime = DateTime.MinValue;
    }
}

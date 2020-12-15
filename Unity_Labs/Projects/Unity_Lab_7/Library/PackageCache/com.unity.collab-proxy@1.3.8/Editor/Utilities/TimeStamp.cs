using System;
using System.Globalization;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Settings;

namespace Unity.Cloud.Collaborate.Utilities
{
    /// <summary>
    /// Static class that presents methods to provide timestamps for the UI.
    /// </summary>
    static class TimeStamp
    {
        /// <summary>
        /// Bool to decide whether timestamps should be exact values or relative values.
        /// </summary>
        public static bool UseRelativeTimeStamps =>
            CollabSettingsManager.Get(CollabSettings.settingRelativeTimestamp, fallback: true);

        /// <summary>
        /// Values to translate a number to a string representation.
        /// </summary>
        static readonly string[] k_UnitsMap = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

        /// <summary>
        /// Get the localized or relative timestamp for the given DateTime based on the current settings.
        /// </summary>
        /// <param name="dateTime">DateTime to convert.</param>
        /// <returns>String representation of the given DateTime.</returns>
        [NotNull]
        public static string GetTimeStamp(DateTimeOffset dateTime)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return UseRelativeTimeStamps
                ? GetElapsedTime(dateTime)
                : GetLocalisedTimeStamp(dateTime);
        }

        /// <summary>
        /// Get the localised timestamp for the given DateTime.
        /// </summary>
        /// <param name="dateTime">DateTime to convert.</param>
        /// <returns>Localised string representation of the given DateTime.</returns>
        [NotNull]
        public static string GetLocalisedTimeStamp(DateTimeOffset dateTime)
        {
            return dateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);
        }

        // Original credit: https://codereview.stackexchange.com/questions/93239/get-elapsed-time-as-human-friendly-string
        /// <summary>
        ///    Convert a DateTime into a relative timestamp.
        /// </summary>
        /// <param name="dateTime">Datetime to calculate the timestamp from.</param>
        /// <returns>Relative timestamp for the given DateTime.</returns>
        [NotNull]
        static string GetElapsedTime(DateTimeOffset dateTime)
        {
            var offset = DateTimeOffset.Now.Subtract(dateTime);

            // The trick: make variable contain date and time representing the desired timespan,
            // having +1 in each date component.
            var date = DateTimeOffset.MinValue + offset;

            return ProcessPeriod(date.Year - 1, date.Month - 1, "year")
                ?? ProcessPeriod(date.Month - 1, date.Day - 1, "month")
                ?? ProcessPeriod(date.Day - 1, date.Hour, "day", "Yesterday")
                ?? ProcessPeriod(date.Hour, date.Minute, "hour")
                ?? ProcessPeriod(date.Minute, date.Second, "minute")
                ?? ProcessPeriod(date.Second, 0, "second")
                ?? "Right now";
        }

        // Original credit: https://codereview.stackexchange.com/questions/93239/get-elapsed-time-as-human-friendly-string
        /// <summary>
        /// Output the string representation for the given time frame. If it's not in that time frame, it returns null.
        /// </summary>
        /// <param name="value">Bigger time value.</param>
        /// <param name="subValue">Smaller time value eg: minutes if value is hours.</param>
        /// <param name="name">Name of the period.</param>
        /// <param name="singularName">Name for period that is singular. Null if it's not singular eg: yesterday.</param>
        /// <returns>String representation of the period, or null if it's outside of it.</returns>
        [CanBeNull]
        static string ProcessPeriod(int value, int subValue, string name, string singularName = null)
        {
            // If the value is less than this time frame, skip.
            if (value == 0)
            {
                return null;
            }

            // If a multiple of this time frame eg: 20 minutes.
            if (value != 1)
            {
                // Convert values specified to string numbers.
                var stringValue = value <k_UnitsMap.Length ? k_UnitsMap[value] : value.ToString();
                return subValue == 0
                    ? $"{stringValue.FirstCharToUpper()} {name}s ago"
                    : $"About {stringValue} {name}s ago";
            }

            // Special case for one-off names eg: yesterday.
            if (!string.IsNullOrEmpty(singularName))
            {
                return singularName;
            }

            // Singular time frame eg: an hour, a minute.
            var articleSuffix = name[0] == 'h' ? "n" : string.Empty;
            return subValue == 0
                ? $"A{articleSuffix} {name} ago"
                : $"About a{articleSuffix} {name} ago";
        }
    }
}

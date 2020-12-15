using System;

namespace Unity.Cloud.Collaborate.Utilities
{
    static class StringUtility
    {
        public static string TrimAndToLower(string value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Trim().ToLower();
        }
    }
}

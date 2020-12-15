using System;
using System.Linq;

namespace Unity.Cloud.Collaborate.Utilities
{
    static class ExtensionMethods
    {
        // Credit: https://stackoverflow.com/a/4405876
        /// <summary>
        /// Take the first letter of the string and capitalise it.
        /// </summary>
        /// <param name="input">String to work with.</param>
        /// <returns>String with first letter capitalised.</returns>
        /// <exception cref="ArgumentNullException">If string is null.</exception>
        /// <exception cref="ArgumentException">If string is empty.</exception>
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}

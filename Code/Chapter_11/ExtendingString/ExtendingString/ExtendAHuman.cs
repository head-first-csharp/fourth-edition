using System;
using System.Collections.Generic;
using System.Text;

namespace AmazingExtensions
{
    public static class ExtendAHuman {
        public static bool IsDistressCall(this string s) {
            if (s.Contains("Help!"))
                return true;
            else
                return false;
        }
    }
}
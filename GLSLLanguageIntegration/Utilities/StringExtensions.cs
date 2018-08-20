using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Utilities
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string value, StringComparison comparison) => source?.IndexOf(value, comparison) >= 0;
    }
}

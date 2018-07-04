using System;
using System.Collections.Generic;
using System.Linq;

namespace GLSLLanguageIntegration.Utilities
{
    public static class LINQExtensions
    {
        public static IEnumerable<T> Yield<T>(this T source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            yield return source;
        }

        public static IEnumerable<T> YieldOrDefault<T>(this T source)
        {
            if (source != null)
            {
                yield return source;
            }
        }

        public static IEnumerable<T> Subset<T>(this IEnumerable<T> source, int startIndex)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex) + " cannot be negative");

            return source.Skip(startIndex);
        }

        public static IEnumerable<T> Subset<T>(this IEnumerable<T> source, int startIndex, int length)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex) + " cannot be negative");
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length) + " cannot be negative");

            return source.Skip(startIndex).Take(source.Count() - startIndex);
        }
    }
}

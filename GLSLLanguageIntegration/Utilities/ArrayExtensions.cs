using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLLanguageIntegration.Utilities
{
    public static class ArrayExtensions
    {
        public static T[] Subset<T>(this T[] source, int startIndex)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex) + " cannot be negative");

            return source.Subset(startIndex, source.Length - startIndex);
        }

        public static T[] Subset<T>(this T[] source, int startIndex, int length)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex) + " cannot be negative");
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length) + " cannot be negative");

            var result = new T[length];

            for (var i = 0; i < length; i++)
            {
                result[i] = source[i + startIndex];
            }

            return result;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace MaiziWPF.Common
{
    public static class StringExtensions
    {
        public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                return string.Empty;
            return string.Join(separator, source.Select(s => s?.ToString()));
        }
    }
}
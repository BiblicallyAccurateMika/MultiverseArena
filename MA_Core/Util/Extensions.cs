using System.Diagnostics.Contracts;

namespace MA_Core.Util;

public static class Extensions
{
    [Pure]
    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return !source.Any(predicate);
    }
}
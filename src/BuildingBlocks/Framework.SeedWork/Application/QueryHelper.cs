using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public static class QueryHelper
{
    public static bool TryParse<T>(string? value, IFormatProvider? provider, out T query) where T : IQuery
    {
        if (value is null)
        {
            query = (T)Activator.CreateInstance(typeof(T), null, null, Array.Empty<string>(), Array.Empty<string>())!;
            return false;
        }

        var matches = Regex.Matches(value, "(?<key>[^=]+)=(?<value>[^&]+)&?");

        query = (T)Activator.CreateInstance(typeof(T),
            int.TryParse(matches.Where(m => m.Groups["key"].Value == "offset")
                .Select(m => m.Groups["value"].Value).LastOrDefault(), out var offset)
                ? offset
                : null,
            int.TryParse(matches.Where(m => m.Groups["key"].Value == "limit")
                .Select(m => m.Groups["value"].Value).LastOrDefault(), out var limit)
                ? limit
                : null,
            matches.Where(m => m.Groups["key"].Value == "sort").Select(m => m.Groups["value"].Value).ToArray(),
            matches.Where(m => m.Groups["key"].Value == "filter").Select(m => m.Groups["value"].Value).ToArray()
        )!;
        return true;
    }
}

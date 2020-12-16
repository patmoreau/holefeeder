using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, IReadOnlyList<string> sort)
        {
            if (sort is null || !sort.Any())
                return query;

            IOrderedQueryable<T> q = null;
            foreach (Match match in Regex.Matches(string.Join(";", sort), @"(?<desc>-{0,1})(?<field>\w+)"))
            {
                if (match.Groups["field"] == null)
                    continue;

                var asc = string.IsNullOrWhiteSpace(match.Groups["desc"]?.Value);
                var field = match.Groups["field"].Value;

                var propertyInfo = typeof(T).GetProperties()
                    .SingleOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo is null)
                    throw new HolefeederInvalidPropertyNameException(field);

                if (q == null)
                {
                    q = asc
                        ? query.OrderBy(x => propertyInfo.GetValue(x))
                        : query.OrderByDescending(x => propertyInfo.GetValue(x));
                }
                else
                {
                    q = asc
                        ? q.ThenBy(x => propertyInfo.GetValue(x))
                        : q.ThenByDescending(x => propertyInfo.GetValue(x));
                }
            }
            return q;
        }
    }
}

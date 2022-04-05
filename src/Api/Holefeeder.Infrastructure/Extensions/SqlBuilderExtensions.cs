using System.Text.RegularExpressions;

using Dapper;

using Holefeeder.Application.SeedWork;

namespace Holefeeder.Infrastructure.Extensions;

public static class SqlBuilderExtensions
{
    private static readonly IDictionary<string, string> _operators =
        new Dictionary<string, string>
        {
            {":eq:", "="},
            {":ne:", "<>"},
            {":gt:", ">"},
            {":ge:", ">="},
            {":lt:", "<"},
            {":le:", "<="}
        };

    public static SqlBuilder Filter(this SqlBuilder query, IReadOnlyList<string> filter)
    {
        if (!filter.Any())
        {
            return query;
        }

        var parameters = new DynamicParameters();
        foreach (var f in filter)
        {
            var match = Regex.Match(f, QueryParams.FILTER_PATTERN);
            var property = match.Groups["property"].Value;
            var op = match.Groups["operator"].Value;
            var value = match.Groups["value"].Value;

            query.Where($"{property} {_operators[op]} @{property}");

            if (Guid.TryParse(value, out var guidValue))
            {
                parameters.Add($"{property}", guidValue);
            }
            else if (bool.TryParse(value, out var boolValue))
            {
                parameters.Add($"{property}", boolValue);
            }
            else
            {
                parameters.Add($"{property}", value);
            }
        }

        query.AddParameters(parameters);
        return query;
    }

    public static SqlBuilder Sort(this SqlBuilder query, IReadOnlyList<string>? sort)
    {
        if (sort is null || !sort.Any())
        {
            return query;
        }

        foreach (Match match in Regex.Matches(string.Join(";", sort), QueryParams.SORT_PATTERN))
        {
            var sortType = string.IsNullOrWhiteSpace(match.Groups["desc"].Value) ? "ASC" : "DESC";
            var field = match.Groups["field"].Value;

            query.OrderBy($"{field} {sortType}");
        }

        return query;
    }
}

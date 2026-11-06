using System.Globalization;
using System.Text.RegularExpressions;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core.Search;

internal static partial class SearchQueryParser
{
    public static Result<T> ParseOrDefault<T>(string? raw, T defaultValue, Func<int, Result<T>> create, ResultError notANumber)
    {
        if (raw is null)
        {
            return defaultValue;
        }

        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? create(value)
            : notANumber;
    }

    public static Result<T> ParseSortBy<T>(string? sort, T defaultValue, Func<string, SortByDirection, Result<T>> create, ResultError error)
    {
        if (sort is null)
        {
            return defaultValue;
        }

        var match = SortPattern().Match(sort);
        if (!match.Success)
        {
            return error;
        }

        var field = match.Groups["field"].Value;
        var orientation = match.Groups["desc"].Success
            ? SortByDirection.Descending
            : SortByDirection.Ascending;

        return create(field, orientation);
    }

    public static Result<T> ParseFilterBy<T>(string? filter, T defaultValue, Func<string, FilterByOperator, string, Result<T>> create, ResultError error)
    {
        if (filter is null)
        {
            return defaultValue;
        }

        var match = FilterPattern().Match(filter);
        if (!match.Success)
        {
            return error;
        }

        var field = match.Groups["field"].Value;
        if(!Operators.TryGetValue(match.Groups["operator"].Value, out var op))
        {
            return error;
        }

        var value = match.Groups["value"].Value;

        return create(field, op, value);
    }

    private static readonly Dictionary<string, FilterByOperator> Operators = new(StringComparer.Ordinal)
     {
         ["eq"] = FilterByOperator.Equal,
         ["ne"] = FilterByOperator.NotEqual,
         ["lt"] = FilterByOperator.LessThan,
         ["le"] = FilterByOperator.LessThanOrEqual,
         ["gt"] = FilterByOperator.GreaterThan,
         ["ge"] = FilterByOperator.GreaterThanOrEqual
     };

    [GeneratedRegex(@"^(?<desc>-)?(?<field>\w+)$")]
    private static partial Regex SortPattern();

    [GeneratedRegex(@"^(?<field>\w+):(?<operator>\w+):(?<value>.+)$")]
    private static partial Regex FilterPattern();
}

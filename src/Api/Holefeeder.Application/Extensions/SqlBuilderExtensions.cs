using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Holefeeder.Application.SeedWork;

namespace Holefeeder.Application.Extensions;

internal static class SqlBuilderExtensions
{
    private static readonly IDictionary<string, string> _operators =
        new Dictionary<string, string>
        {
            {":eq:", "=="},
            {":ne:", "!="},
            {":gt:", ">"},
            {":ge:", ">="},
            {":lt:", "<"},
            {":le:", "<="}
        };

    public static IQueryable<T> Filter<T>(this IQueryable<T> query, IReadOnlyList<string> filter)
    {
        if (!filter.Any())
        {
            return query;
        }

        foreach (var f in filter)
        {
            var match = Regex.Match(f, QueryParams.FILTER_PATTERN);
            var property = match.Groups["property"].Value;
            var op = match.Groups["operator"].Value;
            var value = match.Groups["value"].Value;

            query = query.Where(BuildPredicate<T>(property, _operators[op], value));
        }

        return query;
    }

    public static IQueryable<T> Sort<T>(this IQueryable<T> query, IReadOnlyList<string> sort)
    {
        if (!sort.Any())
        {
            return query;
        }

        var sorts = new List<string>();

        foreach (Match match in Regex.Matches(string.Join(";", sort), QueryParams.SORT_PATTERN))
        {
            var asc = string.IsNullOrWhiteSpace(match.Groups["desc"].Value) ? "ASC" : "DESC";
            var field = match.Groups["field"].Value;

            sorts.Add($"{field} {asc}");
        }

        var sortString = string.Join(", ", sorts);
        return query.OrderBy(sortString);
    }

    public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
        var body = MakeComparison(left, comparison, value);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression MakeComparison(Expression left, string comparison, string value)
    {
        switch (comparison)
        {
            case "==":
                return MakeBinary(ExpressionType.Equal, left, value);
            case "!=":
                return MakeBinary(ExpressionType.NotEqual, left, value);
            case ">":
                return MakeBinary(ExpressionType.GreaterThan, left, value);
            case ">=":
                return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
            case "<":
                return MakeBinary(ExpressionType.LessThan, left, value);
            case "<=":
                return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
            case "Contains":
            case "StartsWith":
            case "EndsWith":
                return Expression.Call(MakeString(left), comparison, Type.EmptyTypes,
                    Expression.Constant(value, typeof(string)));
            default:
                throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
        }
    }

    private static Expression MakeString(Expression source)
    {
        return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
    }

    private static Expression MakeBinary(ExpressionType type, Expression left, string value)
    {
        object? typedValue = value;
        if (left.Type != typeof(string))
        {
            if (string.IsNullOrEmpty(value))
            {
                typedValue = null;
                if (Nullable.GetUnderlyingType(left.Type) == null)
                    left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
            }
            else
            {
                var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                    valueType == typeof(Guid) ? Guid.Parse(value) :
                    Convert.ChangeType(value, valueType, CultureInfo.InvariantCulture);
            }
        }

        var right = Expression.Constant(typedValue, left.Type);
        return Expression.MakeBinary(type, left, right);
    }
}

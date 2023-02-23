using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Holefeeder.Application.SeedWork;

namespace Holefeeder.Application.Extensions;

internal static class SqlBuilderExtensions
{
    private static readonly IDictionary<string, string> Operators =
        new Dictionary<string, string>
        {
            {":eq:", "=="},
            {":ne:", "!="},
            {":gt:", ">"},
            {":ge:", ">="},
            {":lt:", "<"},
            {":le:", "<="}
        };

    public static IQueryable<T> Filter<T>(this IQueryable<T> query, IReadOnlyCollection<string> filter)
    {
        if (!filter.Any())
        {
            return query;
        }

        foreach (string f in filter)
        {
            Match match = Regex.Match(f, QueryParams.FILTER_PATTERN);
            string property = match.Groups["property"].Value;
            string op = match.Groups["operator"].Value;
            string value = match.Groups["value"].Value;

            query = query.Where(BuildPredicate<T>(property, Operators[op], value));
        }

        return query;
    }

    public static IQueryable<T> Sort<T>(this IQueryable<T> query, IReadOnlyCollection<string> sort)
    {
        if (!sort.Any())
        {
            return query;
        }

        List<string> sorts = new List<string>();

        foreach (Match match in Regex.Matches(string.Join(";", sort), QueryParams.SORT_PATTERN))
        {
            string asc = string.IsNullOrWhiteSpace(match.Groups["desc"].Value) ? "ASC" : "DESC";
            string field = match.Groups["field"].Value;

            sorts.Add($"{field} {asc}");
        }

        string sortString = string.Join(", ", sorts);
        return query.OrderBy(sortString);
    }

    public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
        Expression body = MakeComparison(left, comparison, value);
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

    private static Expression MakeString(Expression source) => source.Type == typeof(string)
        ? source
        : Expression.Call(source, "ToString", Type.EmptyTypes);

    private static Expression MakeBinary(ExpressionType type, Expression left, string value)
    {
        object? typedValue = value;
        if (left.Type != typeof(string))
        {
            if (string.IsNullOrEmpty(value))
            {
                typedValue = null;
                if (Nullable.GetUnderlyingType(left.Type) == null)
                {
                    left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                }
            }
            else
            {
                Type valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                    valueType == typeof(Guid) ? Guid.Parse(value) :
                    Convert.ChangeType(value, valueType, CultureInfo.InvariantCulture);
            }
        }

        ConstantExpression right = Expression.Constant(typedValue, left.Type);
        return Expression.MakeBinary(type, left, right);
    }
}

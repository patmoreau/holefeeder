using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Extensions
{
    public static class IMongoDbExtensions
    {
        const string FILTER_PATTERN = @"^(?<property>\w+)(?<operator>[=<>!]{1,2})(?<value>.*)$";

        public static IMongoQueryable<T> Filter<T>(this IMongoQueryable<T> query, IEnumerable<string> filter)
        {
            if (filter is null || !filter.Any())
            {
                return query;
            }

            var q = query;
            foreach (var f in filter)
            {
                var match = Regex.Match(f, FILTER_PATTERN);
                var property = match.Groups["property"]?.Value;
                var op = match.Groups["operator"]?.Value;
                var value = match.Groups["value"]?.Value;

                q = q.Where(property, op, value);
            }
            return q;
        }

        public static IMongoQueryable<T> Sort<T>(this IMongoQueryable<T> query, IEnumerable<string> sort)
        {
            if (sort is null || !sort.Any())
            {
                return query;
            }

            var q = query;
            var sortCount = 0;
            foreach (Match match in Regex.Matches(string.Join(";", sort), @"(?<desc>-{0,1})(?<field>\w+)"))
            {
                if (match.Groups["field"] != null)
                {
                    var asc = string.IsNullOrWhiteSpace(match.Groups["desc"]?.Value);
                    var field = match.Groups["field"].Value;

                    q = (sortCount++ == 0) ? q.OrderBy(field, asc) : q.ThenBy(field, asc);
                }
            }
            return q;
        }

        public static IMongoQueryable<T> Offset<T>(this IMongoQueryable<T> query, int? offset) => offset.HasValue ? query.Skip(offset.Value) : query;

        public static IMongoQueryable<T> Limit<T>(this IMongoQueryable<T> query, int? limit) => limit.HasValue ? query.Take(limit.Value) : query;

        private static IMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> source, string propertyName, bool isAscending) =>
            source.OrderByThenBy("OrderBy", propertyName, isAscending);

        private static IMongoQueryable<T> ThenBy<T>(this IMongoQueryable<T> source, string propertyName, bool isAscending) =>
            source.OrderByThenBy("ThenBy", propertyName, isAscending);

        private static IMongoQueryable<T> OrderByThenBy<T>(this IMongoQueryable<T> source, string method, string propertyName, bool isAscending)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            var parameter = Expression.Parameter(source.ElementType, string.Empty);

            var propertyInfo = source.ElementType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            string methodName = isAscending ? method : ($"{method}Descending");

            MethodCallExpression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                new Type[] { source.ElementType, property.Type }, source.Expression, Expression.Quote(lambda));
            return (IMongoQueryable<T>)source.Provider.CreateQuery<T>(methodCallExpression);
        }

        private static IMongoQueryable<T> Where<T>(this IMongoQueryable<T> source, string propertyName, string op, string value)
        {
            var parameter = Expression.Parameter(source.ElementType, string.Empty);

            var propertyInfo = source.ElementType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var left = Expression.Property(parameter, propertyInfo);

            var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
            var v = converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);

            var right = Expression.Constant(v, propertyInfo.PropertyType);
            var comparison = OperatorExpression(left, op, right);

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            return source.Where(lambda);
        }

        private static Expression OperatorExpression(Expression left, string op, Expression right)
        {
            switch (op)
            {
                case "=":
                    return Expression.Equal(left, right);
                case "<":
                    return Expression.LessThan(left, right);
                case "<=":
                case "=<":
                    return Expression.LessThanOrEqual(left, right);
                case ">":
                    return Expression.GreaterThan(left, right);
                case ">=":
                case "=>":
                    return Expression.GreaterThanOrEqual(left, right);
                case "!=":
                    return Expression.NotEqual(left, right);
                default:
                    throw new ArgumentException($"Invalid operator '{op}'");
            }
        }
    }
}

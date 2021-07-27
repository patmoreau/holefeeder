﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Dapper;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace Framework.Dapper.SeedWork.Extensions
{
    public static class SqlBuilderExtensions
    {
        public static SqlBuilder Filter(this SqlBuilder query, IReadOnlyList<string> filter)
        {
            if (filter is null || !filter.Any())
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
                else
                {
                    parameters.Add($"{property}", value);
                }
            }

            query.AddParameters(parameters);
            return query;
        }

        public static SqlBuilder Sort(this SqlBuilder query, IReadOnlyList<string> sort)
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

        private static readonly IDictionary<string, string> _operators =
            new Dictionary<string, string>
            {
                { ":eq:", "=" },
                { ":ne:", "<>" },
                { ":gt:", ">" },
                { ":ge:", ">=" },
                { ":lt:", "<" },
                { ":le:", "<=" },
            };
    }
}
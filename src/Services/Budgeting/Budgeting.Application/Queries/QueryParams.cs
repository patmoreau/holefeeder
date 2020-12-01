using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class QueryParams
    {
        public int Offset { get; }
        public int Limit { get; }
        public IReadOnlyList<string> Sort { get; }
        public IReadOnlyList<string> Filter { get; }

        public QueryParams(int offset, int limit, IEnumerable<string> sort, IEnumerable<string> filter)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), @"offset cannot be negative");
            }

            if (limit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), @"limit must be positive");
            }

            Offset = offset;
            Limit = limit;
            Sort = sort is null ? ImmutableArray.Create<string>() : ImmutableArray.CreateRange(sort);
            Filter = filter is null ? ImmutableArray.Create<string>() : ImmutableArray.CreateRange(filter);
        }

        public static QueryParams Empty => new QueryParams(0, int.MaxValue, null, null);
    }
}

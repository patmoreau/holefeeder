using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public readonly struct QueryParams
    {
        public int Offset { get; }
        public int Limit { get; }
        public IReadOnlyList<string> Sort { get; }
        public IReadOnlyList<string> Filter { get; }

        public QueryParams(int offset, int limit, IEnumerable<string> sort, IEnumerable<string> filter)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset),
                    Application.QueryParams_QueryParams_Value_cannot_be_negative);
            }

            if (limit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit),
                    Application.QueryParams_QueryParams_Value_cannot_be_negative);
            }

            Offset = offset;
            Limit = limit;
            Sort = sort is null ? ImmutableArray.Create<string>() : ImmutableArray.CreateRange(sort);
            Filter = filter is null ? ImmutableArray.Create<string>() : ImmutableArray.CreateRange(filter);
        }

        public static QueryParams Empty => new QueryParams(0, int.MaxValue, null, null);
    }
}

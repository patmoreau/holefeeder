using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application
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

            sort.ThrowIfNull(nameof(sort));
            filter.ThrowIfNull(nameof(filter));

            Offset = offset;
            Limit = limit;
            Sort = ImmutableArray.CreateRange(sort);
            Filter = ImmutableArray.CreateRange(filter);
        }

        public static QueryParams Empty => new QueryParams(DefaultOffset, DefaultLimit, DefaultSort, DefaultFilter);

        public static readonly int DefaultOffset = 0;
        public static readonly int DefaultLimit = int.MaxValue;
        public static readonly IReadOnlyList<string> DefaultSort = ImmutableArray.Create<string>(); 
        public static readonly IReadOnlyList<string> DefaultFilter = ImmutableArray.Create<string>(); 
        
    }
}

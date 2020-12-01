using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Common
{
    public class QueryParams
    {
        public int? Offset { get; }
        public int? Limit { get; }
        public IReadOnlyList<string> Sort { get; }
        public IReadOnlyList<string> Filter { get; }

        public QueryParams()
        {
            Sort = ImmutableArray.Create<string>();
            Filter = ImmutableArray.Create<string>();
        }

        public QueryParams(int? offset, int? limit, IEnumerable<string> sort, IEnumerable<string> filter)
        {
            Offset = offset;
            Limit = limit;
            Sort = sort is null ? ImmutableArray.Create<string>() : ImmutableArray.CreateRange(sort);
            Filter = filter is null ? ImmutableArray.Create<string>() : ImmutableArray.CreateRange(filter);
        }

        public static QueryParams Empty => new QueryParams();
    }
}

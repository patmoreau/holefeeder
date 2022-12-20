using System.Collections.Immutable;

namespace Holefeeder.Application.SeedWork;

public class QueryParams
{
    public const string FILTER_PATTERN = @"(?<property>\w+)(?<operator>:(eq|ne|lt|le|gt|ge):)(?<value>.*)";
    public const string SORT_PATTERN = @"(?<desc>-{0,1})(?<field>\w+)";

    public const int DEFAULT_OFFSET = 0;
    public const int DEFAULT_LIMIT = int.MaxValue;
    public static readonly IReadOnlyCollection<string> DefaultSort = ImmutableArray.Create<string>();
    public static readonly IReadOnlyCollection<string> DefaultFilter = ImmutableArray.Create<string>();

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

        if (sort is null)
        {
            throw new ArgumentNullException(nameof(sort));
        }

        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        Offset = offset;
        Limit = limit;
        Sort = ImmutableArray.CreateRange(sort);
        Filter = ImmutableArray.CreateRange(filter);
    }

    public int Offset { get; }
    public int Limit { get; }
    public IReadOnlyCollection<string> Sort { get; }
    public IReadOnlyCollection<string> Filter { get; }

    public static QueryParams Empty => new(DEFAULT_OFFSET, DEFAULT_LIMIT, DefaultSort, DefaultFilter);

    public static QueryParams Create(IRequestQuery requestQuery)
    {
        if (requestQuery == null)
        {
            throw new ArgumentNullException(nameof(requestQuery));
        }

        return new QueryParams(requestQuery.Offset, requestQuery.Limit, requestQuery.Sort, requestQuery.Filter);
    }
}

namespace Holefeeder.Ui.Common.Models;

public class PagingInfo<T>
{
    public required int TotalCount { get; init; }
    public required IReadOnlyCollection<T> Items { get; init; }
}

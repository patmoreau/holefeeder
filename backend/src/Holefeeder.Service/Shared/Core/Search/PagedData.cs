namespace Holefeeder.Service.Shared.Core.Search;

internal record PagedData<T>(int Total, IReadOnlyList<T> Items);

namespace Holefeeder.Application.Features.StoreItems.Queries;

public static class GetStoreItems
{
    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter);

    internal record Response(Guid Id, string Code, string Data);
}

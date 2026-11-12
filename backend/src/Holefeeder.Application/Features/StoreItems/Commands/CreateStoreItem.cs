namespace Holefeeder.Application.Features.StoreItems.Commands;

public static class CreateStoreItem
{
    public record Request(string Code, string Data);
}

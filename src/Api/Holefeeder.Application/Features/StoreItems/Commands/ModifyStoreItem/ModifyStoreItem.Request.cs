namespace Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

internal record Request(Guid Id, string Data) : IRequest<Unit>;

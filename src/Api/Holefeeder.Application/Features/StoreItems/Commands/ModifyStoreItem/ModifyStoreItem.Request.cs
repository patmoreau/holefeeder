using Holefeeder.Application.SeedWork;

namespace Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

internal record Request(Guid Id, string Data) : ICommandRequest<Unit>;

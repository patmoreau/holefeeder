using Holefeeder.Application.SeedWork;

using MediatR;

namespace Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

internal record Request(string Code, string Data) : IRequest<Guid>, IStoreItemRequest;

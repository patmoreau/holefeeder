using System;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries
{
    public record GetStoreItemQuery(Guid Id) : IRequest<StoreItemViewModel>;

}

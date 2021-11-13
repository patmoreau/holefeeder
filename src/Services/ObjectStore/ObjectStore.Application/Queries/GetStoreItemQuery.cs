using System;

using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public record GetStoreItemQuery(Guid Id) : IRequest<StoreItemViewModel?>;

}

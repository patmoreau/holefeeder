using System;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemQuery : IRequest<StoreItemViewModel>
    {
        public Guid Id { get; init; }
    }
}

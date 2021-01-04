using System;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemQuery : IRequest<StoreItemViewModel>
    {
        public Guid UserId { get; }
        public Guid Id { get; }

        public GetStoreItemQuery(Guid userId, Guid id)
        {
            UserId = userId;
            Id = id;
        }
    }
}

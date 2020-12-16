using System;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext
{
    public class StoreItem : Entity, IAggregateRoot
    {
        public string Code { get; }
        
        public string Data { get; }
        
        public Guid UserId { get; }
        
        public static StoreItem Create(string code, string data, Guid userId)
        {
            return new StoreItem(Guid.NewGuid(), code, data, userId);
        }

        public StoreItem(Guid id, string code, string data, Guid userId)
        {
            Id = id.ThrowIfNullOrDefault(nameof(id));
            Code = code.ThrowIfNullOrEmpty(nameof(code));
            Data = data.ThrowIfNullOrEmpty(nameof(data));
            UserId = userId.ThrowIfNullOrDefault(nameof(userId));
        }
    }
}

using System;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

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
            if (id == default)
            {
                throw new ObjectStoreDomainException("Id is required");
            }
            Id = id;

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ObjectStoreDomainException("Code is required");
            }
            Code = code;
            
            Data = data;

            if (userId == default)
            {
                throw new ObjectStoreDomainException("UserId is required");
            }
            UserId = userId;
        }

        public StoreItem SetData(string data)
        {
            return new StoreItem(Id, Code, data, UserId);
        }
    }
}

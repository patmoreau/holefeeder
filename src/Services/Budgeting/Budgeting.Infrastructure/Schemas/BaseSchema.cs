using System;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
{
    public abstract class BaseSchema
    {
        public string MongoId { get; set; }
        
        public Guid Id { get; set; }
    }
}

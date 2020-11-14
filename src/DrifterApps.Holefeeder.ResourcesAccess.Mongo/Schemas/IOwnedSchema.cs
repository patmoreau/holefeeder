using System;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    public interface IOwnedSchema
    {
        public Guid UserId { get; set; }
    }
}

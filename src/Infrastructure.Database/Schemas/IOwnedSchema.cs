using System;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Schemas
{
    public interface IOwnedSchema
    {
        public Guid UserId { get; set; }
    }
}

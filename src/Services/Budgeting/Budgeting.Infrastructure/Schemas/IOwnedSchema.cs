using System;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
{
    public interface IOwnedSchema
    {
        public Guid UserId { get; set; }
    }
}

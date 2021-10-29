using System;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
{
    public class AccountSchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "accounts";
        
        public AccountType Type { get; set; }

        public string Name { get; set; }

        public bool Favorite { get; set; }

        public decimal OpenBalance { get; set; }

        public DateTime OpenDate { get; set; }

        public string Description { get; set; }

        public bool Inactive { get; set; }

        public Guid UserId { get; set; }
    }
}

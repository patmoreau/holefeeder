using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities
{
    [Table("accounts")]
    public record AccountEntity : EntityRoot
    {
        public const string SCHEMA = "accounts";
        
        public AccountType Type { get; init; }

        public string Name { get; init; }

        public bool Favorite { get; init; }

        public decimal OpenBalance { get; init; }

        public DateTime OpenDate { get; init; }

        public string Description { get; init; }

        public bool Inactive { get; init; }

        [Key]
        public Guid UserId { get; init; }
    }
}

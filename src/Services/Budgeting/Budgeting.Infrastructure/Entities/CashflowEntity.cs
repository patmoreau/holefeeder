using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities
{
    [Table("cashflows")]
    public record CashflowEntity : EntityRoot
    {
        public const string SCHEMA = "cashflows";

        public DateTime EffectiveDate { get; init; }

        public DateIntervalType IntervalType { get; init; }

        public int Frequency { get; init; }

        public int Recurrence { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; }

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public bool Inactive { get; init; }

        public string[] Tags { get; init; }

        [Key]
        public Guid UserId { get; init; }
        
        [NotMapped]
        public DateTime? LastPaidDate { get; init; }
        
        [NotMapped]
        public DateTime? LastCashflowDate { get; init; }

        [NotMapped]
        public AccountEntity Account { get; init; }
        
        [NotMapped]
        public CategoryEntity Category { get; init; }
    }
}

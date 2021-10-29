using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities
{
    [Table("transactions")]
    public record TransactionEntity : EntityRoot
    {
        public const string SCHEMA = "transactions";

        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; }

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public Guid? CashflowId { get; init; }

        public DateTime? CashflowDate { get; init; }

        public string[] Tags { get; init; }

        [Key]
        public Guid UserId { get; init; }
        
        [NotMapped]
        public AccountEntity Account { get; init; }
        
        [NotMapped]
        public CategoryEntity Category { get; init; }
    }
}

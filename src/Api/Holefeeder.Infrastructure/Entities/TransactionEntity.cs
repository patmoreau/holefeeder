using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Holefeeder.Infrastructure.Entities;

[Table("transactions")]
internal record TransactionEntity : EntityRoot
{
    public DateTime Date { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = null!;

    public Guid AccountId { get; init; }

    public Guid CategoryId { get; init; }

    public Guid? CashflowId { get; init; }

    public DateTime? CashflowDate { get; init; }

    public string Tags { get; init; } = null!;

    [Key]
    public Guid UserId { get; init; }

    [NotMapped]
    public AccountEntity Account { get; init; } = null!;

    [NotMapped]
    public CategoryEntity Category { get; init; } = null!;
}

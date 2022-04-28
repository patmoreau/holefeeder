namespace Holefeeder.Application.Features.MyData.Models;

public record MyDataTransactionDto
{
    public Guid Id { get; init; }

    public DateTime Date { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = null!;

    public string[] Tags { get; init; } = Array.Empty<string>();

    public Guid CategoryId { get; init; }

    public Guid AccountId { get; init; }

    public Guid? CashflowId { get; init; }

    public DateTime? CashflowDate { get; init; }
}

namespace Holefeeder.Application.Features.MyData.Models;

public record MyDataTransactionDto
{
    public Guid Id { get; init; }

    public DateOnly Date { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = null!;

#pragma warning disable CA1819
    public string[] Tags { get; init; } = Array.Empty<string>();
#pragma warning restore CA1819

    public Guid CategoryId { get; init; }

    public Guid AccountId { get; init; }

    public Guid? CashflowId { get; init; }

    public DateOnly? CashflowDate { get; init; }
}

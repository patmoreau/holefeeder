using Holefeeder.Domain.Enumerations;

namespace Holefeeder.Application.Features.MyData.Models;

public record MyDataCashflowDto
{
    public Guid Id { get; init; }

    public DateOnly EffectiveDate { get; init; }

    public decimal Amount { get; init; }

    public DateIntervalType IntervalType { get; set; } = null!;

    public int Frequency { get; set; }

    public int Recurrence { get; set; }

    public string Description { get; init; } = null!;

#pragma warning disable CA1819
    public string[] Tags { get; init; } = Array.Empty<string>();
#pragma warning restore CA1819

    public Guid CategoryId { get; init; }

    public Guid AccountId { get; init; }

    public bool Inactive { get; init; }
}

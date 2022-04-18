using System;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;

public record MyDataCashflowDto
{
    public Guid Id { get; init; }

    public DateTime EffectiveDate { get; init; }

    public decimal Amount { get; init; }

    public DateIntervalType IntervalType { get; set; } = null!;

    public int Frequency { get; set; }

    public int Recurrence { get; set; }

    public string Description { get; init; } = null!;

    public string[] Tags { get; init; } = Array.Empty<string>();

    public Guid CategoryId { get; init; }

    public Guid AccountId { get; init; }

    public bool Inactive { get; init; }
}

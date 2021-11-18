using System;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public record MakePurchaseCommand : IRequest<Guid>
{
    public DateTime Date { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = null!;

    public Guid AccountId { get; init; }

    public Guid CategoryId { get; init; }

    public string[] Tags { get; init; } = null!;
}

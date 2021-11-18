using System;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public record TransferCommand : IRequest<Guid>
{
    public DateTime Date { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = null!;

    public Guid FromAccountId { get; init; }

    public Guid ToAccountId { get; init; }
}

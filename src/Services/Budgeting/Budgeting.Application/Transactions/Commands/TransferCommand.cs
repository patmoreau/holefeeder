using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands
{
    public record TransferCommand : IRequest<CommandResult<Guid>>
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; }

        public Guid FromAccountId { get; init; }

        public Guid ToAccountId { get; init; }
    }
}

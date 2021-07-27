using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands
{
    public record PayCashflowCommand : IRequest<CommandResult<Guid>>
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public Guid CashflowId { get; init; }

        public DateTime CashflowDate { get; init; }
    }
}

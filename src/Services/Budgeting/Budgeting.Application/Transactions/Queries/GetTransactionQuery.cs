using System;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries
{
    public record GetTransactionQuery(Guid Id) : IRequest<TransactionViewModel?>;
}

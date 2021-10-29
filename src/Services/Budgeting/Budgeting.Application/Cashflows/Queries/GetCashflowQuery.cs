using System;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries
{
    public record GetCashflowQuery : IRequest<CashflowViewModel>
    {
        public Guid Id { get; init; }
    }
}

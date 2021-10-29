using System;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands
{
    public record OpenAccountCommand : IRequest<CommandResult<Guid>>
    {
        public AccountType Type { get; init; }
        public string Name { get; init; }
        public DateTime OpenDate { get; init; }
        public decimal OpenBalance { get; init; }
        public string Description { get; init; }
    }
}

using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands
{
    public record ModifyAccountCommand : IRequest<CommandResult>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal OpenBalance { get; init; }
        public string Description { get; init; }
    }
}

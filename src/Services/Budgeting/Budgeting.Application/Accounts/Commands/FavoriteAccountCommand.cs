using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands
{
    public record FavoriteAccountCommand : IRequest<CommandResult>
    {
        public Guid Id { get; init; }
        public bool IsFavorite { get; init; }
    }
}

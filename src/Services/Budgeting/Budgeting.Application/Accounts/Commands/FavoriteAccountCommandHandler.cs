using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands
{
    public class FavoriteAccountCommandHandler : IRequestHandler<FavoriteAccountCommand, CommandResult>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<FavoriteAccountCommandHandler> _logger;

        public FavoriteAccountCommandHandler(IAccountRepository repository, ItemsCache cache, ILogger<FavoriteAccountCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }
        public async Task<CommandResult> Handle(FavoriteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _repository.FindByIdAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);
            if (account is null)
            {
                return CommandResult.Create(CommandStatus.NotFound);
            }
            
            account = account with { Favorite = request.IsFavorite};

            _logger.LogInformation("----- Set Favorite - Account: {@Account}", account);

            await _repository.SaveAsync(account, cancellationToken);

            await _repository.UnitOfWork.CommitAsync(cancellationToken);

            return CommandResult.Create(CommandStatus.Ok);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands
{
    public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, CommandResult>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<CloseAccountCommandHandler> _logger;

        public CloseAccountCommandHandler(IAccountRepository repository, ItemsCache cache, ILogger<CloseAccountCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }
        public async Task<CommandResult> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _repository.FindByIdAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);
            if (account is null)
            {
                return CommandResult.Create(CommandStatus.NotFound);
            }
            
            _logger.LogInformation("----- Closing Account - Account: {@Account}", account);

            try
            {
                account.Close();

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return CommandResult.Create(CommandStatus.Ok);
            }
            catch (HolefeederDomainException e)
            {
                return CommandResult.Create(CommandStatus.Conflict, e.Message);
            }
        }
    }
}

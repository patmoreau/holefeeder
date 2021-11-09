using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands
{
    public class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, CommandResult<Guid>>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<OpenAccountCommandHandler> _logger;

        public OpenAccountCommandHandler(IAccountRepository repository, ItemsCache cache,
            ILogger<OpenAccountCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public Task<CommandResult<Guid>> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return FindByNameInternalAsync(request, cancellationToken);
        }

        private async Task<CommandResult<Guid>> FindByNameInternalAsync(OpenAccountCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _repository.FindByNameAsync(request.Name, (Guid)_cache["UserId"], cancellationToken);
                if (exists is not null)
                {
                    throw new ValidationException($"Account name '{request.Name}' already exists");
                }

                var account = Account.Create(request.Type, request.Name, request.OpenBalance, request.OpenDate,
                    request.Description, (Guid)_cache["UserId"]);

                _logger.LogInformation("----- Opening Account - Account: {@Account}", account);

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return CommandResult<Guid>.Create(CommandStatus.Created, account.Id);
            }
            catch (HolefeederDomainException e)
            {
                return CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty, e.Message);
            }
        }
    }
}

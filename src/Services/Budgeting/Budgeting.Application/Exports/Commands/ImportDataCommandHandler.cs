using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Exports.Commands
{
    public class ImportDataCommandHandler : IRequestHandler<ImportDataCommand, CommandResult<int>>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<OpenAccountCommandHandler> _logger;

        public ImportDataCommandHandler(IAccountRepository repository, ItemsCache cache,
            ILogger<OpenAccountCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public Task<CommandResult<int>> Handle(ImportDataCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return ImportDataInternalAsync(request, cancellationToken);
        }

        private async Task<CommandResult<int>> ImportDataInternalAsync(ImportDataCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var count = await ImportAccountsAsync(request, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return CommandResult<int>.Create(CommandStatus.Ok, count);
            }
            catch (HolefeederDomainException e)
            {
                return CommandResult<int>.Create(CommandStatus.BadRequest, 0, e.Message);
            }
        }

        private static JsonProperty GetProperty(JsonElement element, string name) =>
            element.EnumerateObject()
                .Single(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        private async Task<int> ImportAccountsAsync(ImportDataCommand request, CancellationToken cancellationToken)
        {
            var count = 0;
            
            var accounts = request.Data.RootElement.GetProperty("accounts");

            var userId = (Guid)_cache["UserId"];

            foreach (var element in accounts.EnumerateArray())
            {
                Account account = null;
                var id = GetProperty(element, "id").Value.GetGuid();
                var type = Enumeration.FromName<AccountType>(GetProperty(element, "type").Value.GetString());
                var name = GetProperty(element, "name").Value.GetString();
                var favorite = GetProperty(element, "favorite").Value.GetBoolean();
                var openBalance = GetProperty(element, "openBalance").Value.GetDecimal();
                var openDate = GetProperty(element, "openDate").Value.GetDateTime();
                var description = GetProperty(element, "description").Value.GetString();
                var inactive = GetProperty(element, "inactive").Value.GetBoolean();
                
                var exists = await _repository.FindByIdAsync(id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        account = exists with
                        {
                            Type = type,
                            Name = name,
                            Favorite = favorite,
                            OpenBalance = openBalance,
                            OpenDate = openDate,
                            Description = description,
                            Inactive = inactive
                        };
                        _logger.LogInformation("----- Modify Account - Account: {@Account}", account);
                    }
                    else
                    {
                        _logger.LogInformation("----- Ignore Account - Account: {@Account}", exists);
                    }
                }
                else
                {
                    account = new Account
                    {
                        Id = id,
                        Type = type,
                        Name = name,
                        Favorite = favorite,
                        OpenBalance = openBalance,
                        OpenDate = openDate,
                        Description = description,
                        Inactive = inactive,
                        UserId = userId
                    };
                    _logger.LogInformation("----- Create Account - Account: {@Account}", account);
                }

                if (account is null)
                {
                    continue;
                }

                count++;
                await _repository.SaveAsync(account, cancellationToken);
            }

            return count;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;

public static class OpenAccount
{
    public record Request(AccountType Type, string Name, DateTime OpenDate, decimal OpenBalance, string Description)
        : IRequest<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>, IValidateable;

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(command => command.Type).NotNull();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
            RuleFor(command => command.OpenDate).NotNull().NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult> CreateResponse(
            ValidationResult result)
        {
            return new ValidationErrorsRequestResult(result.ToDictionary());
        }
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        private readonly ItemsCache _cache;
        private readonly ILogger<Handler> _logger;
        private readonly IAccountRepository _repository;

        public Handler(IAccountRepository repository, ItemsCache cache,
            ILogger<Handler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var exists = await _repository.FindByNameAsync(request.Name, (Guid)_cache["UserId"], cancellationToken);
            if (exists is not null)
            {
                return new ValidationErrorsRequestResult(new Dictionary<string, string[]>
                {
                    {nameof(request.Name), new[] {$"'{request.Name}' already exists."}}
                });
            }

            try
            {
                var account = Account.Create(request.Type, request.Name, request.OpenBalance, request.OpenDate,
                    request.Description, (Guid)_cache["UserId"]);

                _logger.LogInformation("----- Opening Account - Account: {@Account}", account);

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return account.Id;
            }
            catch (HolefeederDomainException ex)
            {
                _repository.UnitOfWork.Dispose();
                return new DomainErrorRequestResult(ex.Context, ex.Message);
            }
        }
    }
}

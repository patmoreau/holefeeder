using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;

public static class FavoriteAccount
{
    public record Request(Guid Id, bool IsFavorite)
        : IRequest<OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>,
            IValidateable;

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>
            CreateResponse(ValidationResult result) => new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler
        : IRequestHandler<Request,
            OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
    {
        private readonly IAccountRepository _repository;
        private readonly ItemsCache _cache;
        private readonly ILogger<Handler> _logger;

        public Handler(IAccountRepository repository, ItemsCache cache,
            ILogger<Handler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
            Handle(Request request, CancellationToken cancellationToken)
        {
            var account = await _repository.FindByIdAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);
            if (account is null)
            {
                return new NotFoundRequestResult();
            }

            try
            {
                account = account with {Favorite = request.IsFavorite};

                _logger.LogInformation("----- Set Favorite - Account: {@Account}", account);

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (HolefeederDomainException ex)
            {
                _repository.UnitOfWork.Dispose();
                return new DomainErrorRequestResult(ex.Context, ex.Message);
            }
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public static class DeleteTransaction
{
    public record Request(Guid Id)
        : IRequest<OneOf<ValidationErrorsRequestResult, Unit, NotFoundRequestResult>>, IValidateable;

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, Unit, NotFoundRequestResult>>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
        }

        public OneOf<ValidationErrorsRequestResult, Unit, NotFoundRequestResult> CreateResponse(
            ValidationResult result)
        {
            return new ValidationErrorsRequestResult(result.ToDictionary());
        }
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, Unit, NotFoundRequestResult>>
    {
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(ITransactionRepository transactionRepository, ItemsCache cache,
            ILogger<Handler> logger)
        {
            _transactionRepository = transactionRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, Unit, NotFoundRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var transaction =
                await _transactionRepository.FindByIdAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);
            if (transaction is null)
            {
                return new NotFoundRequestResult();
            }

            _logger.LogInformation("----- Deleting - Transaction: {@Transaction}", transaction);

            await _transactionRepository.DeleteAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

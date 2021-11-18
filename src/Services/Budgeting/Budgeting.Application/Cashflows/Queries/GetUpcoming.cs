using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries;

public static class GetUpcoming
{
    public record Request(DateTime From, DateTime To)
        : IRequest<OneOf<ValidationErrorsRequestResult, ListRequestResult>>, IValidateable;

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, ListRequestResult>>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(command => command.From).NotEmpty();
            RuleFor(command => command.To)
                .NotEmpty()
                .GreaterThanOrEqualTo(command => command.From)
                .WithMessage("");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public OneOf<ValidationErrorsRequestResult, ListRequestResult> CreateResponse(ValidationResult result) =>
            new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler : IRequestHandler<Request, OneOf<ValidationErrorsRequestResult, ListRequestResult>>
    {
        private readonly IUpcomingQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public Handler(IUpcomingQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, ListRequestResult>> Handle(Request query,
            CancellationToken cancellationToken)
        {
            var results =
                (await _repository.GetUpcomingAsync((Guid)_cache["UserId"], query.From, query.To, cancellationToken))
                .ToList();
            return new ListRequestResult(results.Count, results);
        }
    }
}

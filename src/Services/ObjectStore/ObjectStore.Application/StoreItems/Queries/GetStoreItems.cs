using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Http;

using OneOf;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

public static class GetStoreItems
{
    public record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<OneOf<ValidationErrorsRequestResult, ListRequestResult>>, IRequestQuery, IValidateable
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
            => context.ToQueryRequest(((offset, limit, sort, filter) => new Request(offset, limit, sort, filter)));
    }

    public class Validator : QueryValidatorRoot<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, ListRequestResult>>
    {
        public OneOf<ValidationErrorsRequestResult, ListRequestResult> CreateResponse(ValidationResult result) =>
            new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler : IRequestHandler<Request, OneOf<ValidationErrorsRequestResult, ListRequestResult>>
    {
        private readonly IStoreItemsQueriesRepository _itemsQueriesRepository;
        private readonly ItemsCache _cache;

        public Handler(IStoreItemsQueriesRepository itemsQueriesRepository, ItemsCache cache)
        {
            _itemsQueriesRepository = itemsQueriesRepository;
            _cache = cache;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, ListRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var (total, items) =
                await _itemsQueriesRepository.FindAsync((Guid)_cache["UserId"], QueryParams.Create(request),
                    cancellationToken);

            return new ListRequestResult(total, items);
        }
    }
}

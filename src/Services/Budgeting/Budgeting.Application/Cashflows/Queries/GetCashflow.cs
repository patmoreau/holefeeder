using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries;

public static class GetCashflow
{
    public record Request(Guid Id) : IRequest<OneOf<CashflowInfoViewModel, NotFoundRequestResult>>;

    public class Handler : IRequestHandler<Request, OneOf<CashflowInfoViewModel, NotFoundRequestResult>>
    {
        private readonly ItemsCache _cache;
        private readonly ICashflowQueriesRepository _repository;

        public Handler(ICashflowQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<OneOf<CashflowInfoViewModel, NotFoundRequestResult>> Handle(Request query,
            CancellationToken cancellationToken)
        {
            var result = await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken);
            if (result is null)
            {
                return new NotFoundRequestResult();
            }

            return result;
        }
    }
}

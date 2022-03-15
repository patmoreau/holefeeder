using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;

public static class GetAccount
{
    public record Request(Guid Id) : IRequest<OneOf<AccountViewModel, NotFoundRequestResult>>;

    public class Handler : IRequestHandler<Request, OneOf<AccountViewModel, NotFoundRequestResult>>
    {
        private readonly ItemsCache _cache;
        private readonly IAccountQueriesRepository _repository;

        public Handler(IAccountQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<OneOf<AccountViewModel, NotFoundRequestResult>> Handle(Request query,
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

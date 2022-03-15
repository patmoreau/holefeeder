using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Queries;

public static class ExportData
{
    public record Request : IRequest<OneOf<ExportDataDto>>;

    public class Handler
        : IRequestHandler<Request, OneOf<ExportDataDto>>
    {
        private readonly ItemsCache _cache;
        private readonly ILogger<Handler> _logger;
        private readonly IMyDataQueriesRepository _myDataRepository;

        public Handler(
            IMyDataQueriesRepository myDataRepository, ItemsCache cache, ILogger<Handler> logger)
        {
            _myDataRepository = myDataRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ExportDataDto>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("***** Request: {@Request}", request);

            var accounts = await _myDataRepository.ExportAccountsAsync((Guid)_cache["UserId"], cancellationToken);
            var categories = await _myDataRepository.ExportCategoriesAsync((Guid)_cache["UserId"], cancellationToken);
            var cashflows = await _myDataRepository.ExportCashflowsAsync((Guid)_cache["UserId"], cancellationToken);
            var transactions =
                await _myDataRepository.ExportTransactionsAsync((Guid)_cache["UserId"], cancellationToken);

            return new ExportDataDto(accounts.ToImmutableArray(), categories.ToImmutableArray(),
                cashflows.ToImmutableArray(), transactions.ToImmutableArray());
        }
    }
}

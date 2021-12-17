using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Queries;

public static class ImportDataStatus
{
    public record Request(Guid RequestId) : IRequest<OneOf<ImportDataStatusViewModel, NotFoundRequestResult>>;

    public class Handler
        : IRequestHandler<Request, OneOf<ImportDataStatusViewModel, NotFoundRequestResult>>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<Handler> _logger;

        public Handler(IMemoryCache memoryCache, ILogger<Handler> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<OneOf<ImportDataStatusViewModel, NotFoundRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("***** Request: {@Request}", request);
            if (_memoryCache.TryGetValue(request.RequestId, out var status))
            {
                return Task.FromResult<OneOf<ImportDataStatusViewModel, NotFoundRequestResult>>(
                    (ImportDataStatusViewModel)status);
            }

            return Task.FromResult<OneOf<ImportDataStatusViewModel, NotFoundRequestResult>>(
                new NotFoundRequestResult());
        }
    }
}

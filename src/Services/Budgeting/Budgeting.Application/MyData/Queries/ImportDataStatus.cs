using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Queries;

public static class ImportDataStatus
{
    public record Request(Guid RequestId) : IRequest<OneOf<ImportDataStatusDto, NotFoundRequestResult>>;

    public class Handler
        : IRequestHandler<Request, OneOf<ImportDataStatusDto, NotFoundRequestResult>>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<Handler> _logger;

        public Handler(IMemoryCache memoryCache, ILogger<Handler> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<OneOf<ImportDataStatusDto, NotFoundRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("***** Request: {@Request}", request);
            if (_memoryCache.TryGetValue(request.RequestId, out var status))
            {
                return Task.FromResult<OneOf<ImportDataStatusDto, NotFoundRequestResult>>(
                    (ImportDataStatusDto)status);
            }

            return Task.FromResult<OneOf<ImportDataStatusDto, NotFoundRequestResult>>(
                new NotFoundRequestResult());
        }
    }
}

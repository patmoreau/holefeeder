using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Queries;

public class ImportDataStatusQueryHandler
    : IRequestHandler<ImportDataStatusQuery, CommandResult<ImportDataStatusViewModel?>>
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ImportDataStatusQueryHandler> _logger;

    public ImportDataStatusQueryHandler(IMemoryCache memoryCache, ILogger<ImportDataStatusQueryHandler> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<CommandResult<ImportDataStatusViewModel?>> Handle(ImportDataStatusQuery request,
        CancellationToken cancellationToken)
    {
        var status = _memoryCache.Get<CommandResult<ImportDataStatusViewModel?>>(request.RequestId);
        return Task.FromResult(status ?? CommandResult<ImportDataStatusViewModel?>.Create(CommandStatus.NotFound,
            default(ImportDataStatusViewModel), "RequestID not found"));
    }
}

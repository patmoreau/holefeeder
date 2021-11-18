using System;

using DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using Microsoft.Extensions.Caching.Memory;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Commands;

public class ImportDataCommandHandler
    : BackgroundRequestHandler<ImportDataCommand, ImportDataCommandTask, CommandResult<ImportDataStatusViewModel>>
{
    public ImportDataCommandHandler(
        ItemsCache cache,
        IServiceProvider serviceProvider,
        BackgroundWorkerQueue backgroundWorkerQueue,
        IMemoryCache memoryCache) : base(serviceProvider, backgroundWorkerQueue, memoryCache)
    {
        UserId = (Guid)cache["UserId"];
    }

    protected override Guid UserId { get; }
}

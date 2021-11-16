using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public abstract class
    BackgroundRequestHandler<TRequest, TBackgroundTask, TResponse> : IBackgroundRequestHandler<TRequest,
        TBackgroundTask, TResponse>
    where TRequest : IRequest<Guid>
    where TBackgroundTask : IBackgroundTask<TRequest, TResponse>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BackgroundWorkerQueue _backgroundWorkerQueue;
    private readonly IMemoryCache _memoryCache;

    private readonly Guid _id = Guid.NewGuid();

    protected abstract Guid UserId { get; }

    protected BackgroundRequestHandler(IServiceProvider serviceProvider,
        BackgroundWorkerQueue backgroundWorkerQueue, IMemoryCache memoryCache)
    {
        _serviceProvider = serviceProvider;
        _backgroundWorkerQueue = backgroundWorkerQueue;
        _memoryCache = memoryCache;
    }

    public Task<Guid> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
        {
            using var scope = _serviceProvider.CreateScope();

            var task = scope.ServiceProvider.GetRequiredService<TBackgroundTask>();

            await task.Handle(UserId, request, UpdateTaskProgress, token);
        });

        return Task.FromResult(_id);
    }

    private void UpdateTaskProgress(TResponse response)
    {
        _memoryCache.Set(_id, response, TimeSpan.FromHours(1));
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

public abstract class
    BackgroundRequestHandler<TRequest, TBackgroundTask, TResponse> : IBackgroundRequestHandler<TRequest>
    where TRequest : IRequest<RequestResponse>
    where TBackgroundTask : IBackgroundTask<TRequest, TResponse>
{
    private readonly BackgroundWorkerQueue _backgroundWorkerQueue;

    private readonly Guid _id = Guid.NewGuid();
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceProvider _serviceProvider;

    protected BackgroundRequestHandler(IServiceProvider serviceProvider,
        BackgroundWorkerQueue backgroundWorkerQueue, IMemoryCache memoryCache)
    {
        _serviceProvider = serviceProvider;
        _backgroundWorkerQueue = backgroundWorkerQueue;
        _memoryCache = memoryCache;
    }

    protected Guid UserId { get; init; }

    public Task<RequestResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
        {
            using var scope = _serviceProvider.CreateScope();

            var task = scope.ServiceProvider.GetRequiredService<TBackgroundTask>();

            await task.Handle(UserId, request, UpdateTaskProgress, token);
        });

        return Task.FromResult(new RequestResponse(_id));
    }

    private void UpdateTaskProgress(TResponse response)
    {
        _memoryCache.Set(_id, response, TimeSpan.FromHours(1));
    }
}

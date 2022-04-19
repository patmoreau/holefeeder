using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.SeedWork.BackgroundRequest;

public abstract class
    BackgroundRequestHandler<TRequest, TBackgroundTask, TResponse> : IBackgroundRequestHandler<TRequest>
    where TRequest : IRequest<Guid>
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

    public Task<Guid> Handle(TRequest request, CancellationToken cancellationToken)
    {
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

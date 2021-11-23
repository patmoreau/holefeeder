using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

public interface IBackgroundTask<in TRequest, out TResponse>
{
    Task Handle(Guid userId, TRequest request, Action<TResponse> updateProgress, CancellationToken cancellationToken);
}

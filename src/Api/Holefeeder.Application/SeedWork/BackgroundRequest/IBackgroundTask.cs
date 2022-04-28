namespace Holefeeder.Application.SeedWork.BackgroundRequest;

public interface IBackgroundTask<in TRequest, out TResponse>
{
    Task Handle(Guid userId, TRequest request, Action<TResponse> updateProgress, CancellationToken cancellationToken);
}

using MediatR;

namespace Holefeeder.Application.SeedWork.BackgroundRequest;

public interface IBackgroundRequestHandler<in TRequest> : IRequestHandler<TRequest, Guid>
    where TRequest : IRequest<Guid>
{
}

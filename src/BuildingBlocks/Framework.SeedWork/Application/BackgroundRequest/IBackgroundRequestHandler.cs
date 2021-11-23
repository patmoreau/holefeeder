using MediatR;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

public interface IBackgroundRequestHandler<in TRequest> : IRequestHandler<TRequest, RequestResponse>
    where TRequest : IRequest<RequestResponse>
{
}

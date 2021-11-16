using System;

using MediatR;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public interface
    IBackgroundRequestHandler<in TRequest, TBackgroundTask, TResponse> : IRequestHandler<TRequest, Guid>
    where TRequest : IRequest<Guid>
    where TBackgroundTask : IBackgroundTask<TRequest, TResponse>
{
}

using System;

using MediatR;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application
{
    public interface IBackgroundRequestHandler<in TRequest, TBackgroundTask, TResponse> : IRequestHandler<TRequest, CommandResult<Guid>>
        where TRequest : IRequest<CommandResult<Guid>>
        where TBackgroundTask : IBackgroundTask<TRequest, TResponse>
    {
        
    }
}

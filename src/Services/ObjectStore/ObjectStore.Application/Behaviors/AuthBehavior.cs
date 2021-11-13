using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Behaviors
{
    public class AuthBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ItemsCache _itemsCache;
        private readonly Func<IRequestUser> _getRequestUser;

        public AuthBehavior(ItemsCache itemsCache, Func<IRequestUser> getRequestUser)
        {
            _itemsCache = itemsCache;
            _getRequestUser = getRequestUser;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _itemsCache["UserId"] = _getRequestUser().UserId;

            return next();
        }
    }
}

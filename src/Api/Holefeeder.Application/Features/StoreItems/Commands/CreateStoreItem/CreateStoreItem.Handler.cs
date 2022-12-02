using Holefeeder.Application.Context;
using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.StoreItem;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

internal class Handler : IRequestHandler<Request, Guid>
{
    private readonly IUserContext _userContext;
    private readonly StoreItemContext _context;

    public Handler(IUserContext userContext, StoreItemContext context)
    {
        _userContext = userContext;
        _context = context;
    }

    public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
    {
        if (await _context.StoreItems.AsQueryable()
                .AnyAsync(e => e.Code == request.Code, cancellationToken: cancellationToken))
        {
            throw new ObjectStoreDomainException($"Code '{request.Code}' already exists.");
        }
        var storeItem = StoreItem.Create(request.Code, request.Data, _userContext.UserId);

        _context.StoreItems.Add(storeItem);

        return storeItem.Id;
    }
}

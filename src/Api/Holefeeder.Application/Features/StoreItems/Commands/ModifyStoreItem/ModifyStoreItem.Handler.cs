using Holefeeder.Application.Context;
using Holefeeder.Application.Features.StoreItems.Exceptions;
using Holefeeder.Application.SeedWork;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

internal class Handler : IRequestHandler<Request, Unit>
{
    private readonly IUserContext _userContext;
    private readonly StoreItemContext _context;

    public Handler(IUserContext userContext, StoreItemContext context)
    {
        _userContext = userContext;
        _context = context;
    }

    public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
    {
        var storeItem = await _context.StoreItems.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == _userContext.UserId, cancellationToken);
        if (storeItem is null)
        {
            throw new StoreItemNotFoundException(request.Id);
        }

        storeItem = storeItem with {Data = request.Data};

        return Unit.Value;
    }
}

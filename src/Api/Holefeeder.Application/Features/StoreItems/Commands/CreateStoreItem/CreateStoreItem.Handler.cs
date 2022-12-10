using Holefeeder.Application.Context;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.StoreItem;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

internal class Handler : IRequestHandler<Request, Guid>
{
    private readonly IUserContext _userContext;
    private readonly BudgetingContext _context;

    public Handler(IUserContext userContext, BudgetingContext context)
    {
        _userContext = userContext;
        _context = context;
    }

    public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
    {
        if (await _context.StoreItems
                .AnyAsync(e => e.Code == request.Code, cancellationToken: cancellationToken))
        {
            throw new ObjectStoreDomainException($"Code '{request.Code}' already exists.");
        }
        var storeItem = StoreItem.Create(request.Code, request.Data, _userContext.UserId);

        _context.StoreItems.Add(storeItem);

        return storeItem.Id;
    }
}

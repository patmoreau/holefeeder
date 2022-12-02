using Holefeeder.Application.Context;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Application.Behaviors;

internal class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IStoreItemRequest
{
    private readonly StoreItemContext _context;

    public TransactionBehavior(StoreItemContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await _context.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            await _context.SaveChangesAsync(cancellationToken);
            await _context.CommitTransactionAsync(cancellationToken);

            return response;
        }
        catch (DomainException)
        {
            await _context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

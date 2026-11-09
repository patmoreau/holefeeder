using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Queries;

public static class GetStoreItem
{
    public record Request(StoreItemId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => StoreItemId.Create(x)).NotEqual(StoreItemId.Empty);
    }
}

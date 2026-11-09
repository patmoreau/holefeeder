using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Commands;

public static class ModifyStoreItem
{
    public record Request(StoreItemId Id, string Data);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEqual(StoreItemId.Empty);
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}

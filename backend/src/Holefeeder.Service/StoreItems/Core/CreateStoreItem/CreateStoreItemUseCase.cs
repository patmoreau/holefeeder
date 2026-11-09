using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Core.CreateStoreItem;

internal sealed class CreateStoreItemUseCase(IStoreItems storeItems)
{
    public async Task<Result<Id>> HandleAsync(CreateStoreItemRequest request, CancellationToken cancellationToken)
    {
        if (request.User == User.NoUser)
        {
            return StoreItemErrors.UserNotRegistered;
        }

        var itemResult = StoreItem.Create(Id.New(), request.Code, request.Data.Value);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var item = itemResult.Value;

        var saveResult = await storeItems.SaveAsync(request.User, item, cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error;
        }

        return item.Id;
    }
}

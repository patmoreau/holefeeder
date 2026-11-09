using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core.GetStoreItem;

namespace Holefeeder.Service.StoreItems.Core.ModifyStoreItem;

internal sealed class ModifyStoreItemUseCase(IStoreItems storeItems)
{
    public async Task<Result<Nothing>> HandleAsync(ModifyStoreItemRequest request, CancellationToken cancellationToken)
    {
        var itemResult = await storeItems.GetAsync(new GetStoreItemRequest(request.User, request.Id), cancellationToken);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }

        return await storeItems.SaveAsync(request.User, itemResult.Value with { Data = request.Data.Value }, cancellationToken);
    }
}

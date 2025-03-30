using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.StoreItem;

public sealed partial record StoreItem
{
    public static Result<StoreItem> Create(string code, string data, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(CodeValidation(code))
            .Ensure(UserIdValidation(userId));

        return result.OnSuccess(
            () => new StoreItem(StoreItemId.New, code, userId)
            {
                Data = data
            }.ToResult());
    }
}

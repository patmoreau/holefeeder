using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.StoreItem;

public sealed partial record StoreItem
{
    public static Result<StoreItem> Create(string code, string data, UserId userId)
    {
        var result = Result.Validate(CodeValidation(code), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<StoreItem>.Failure(result.Error);
        }

        return Result<StoreItem>.Success(new StoreItem(StoreItemId.New, code, userId)
        {
            Data = data
        });
    }
}

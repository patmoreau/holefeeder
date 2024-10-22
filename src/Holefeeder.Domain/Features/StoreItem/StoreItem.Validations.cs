using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.StoreItem;

public sealed partial record StoreItem
{
    private static Func<Result<Nothing>> CodeValidation(string code) =>
        () => !string.IsNullOrWhiteSpace(code)
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(StoreItemErrors.CodeRequired);

    private static Func<Result<Nothing>> UserIdValidation(Guid id) =>
        () => id != Guid.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(StoreItemErrors.UserIdRequired);
}

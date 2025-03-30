using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.StoreItem;

public sealed partial record StoreItem
{
    private static Func<Result<Nothing>> CodeValidation(string code) =>
        () => !string.IsNullOrWhiteSpace(code)
            ? Nothing.Value
            : StoreItemErrors.CodeRequired;

    private static Func<Result<Nothing>> UserIdValidation(Guid id) =>
        () => id != Guid.Empty
            ? Nothing.Value
            : StoreItemErrors.UserIdRequired;
}

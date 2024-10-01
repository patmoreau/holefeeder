namespace Holefeeder.Domain.Features.StoreItem;

public sealed partial record StoreItem
{
    private static ResultValidation CodeValidation(string code) =>
        ResultValidation.Create(() => !string.IsNullOrWhiteSpace(code), StoreItemErrors.CodeRequired);

    private static ResultValidation UserIdValidation(Guid id) =>
        ResultValidation.Create(() => id != Guid.Empty, StoreItemErrors.UserIdRequired);
}

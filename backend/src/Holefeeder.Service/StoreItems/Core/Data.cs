using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.StoreItems.Core;

internal sealed record Data
{
    private Data(string value) => Value = value;

    public string Value { get; }

    public static Result<Data> Create(string? value) =>
        string.IsNullOrWhiteSpace(value) ? DataErrors.Invalid : new Data(value);

    public static Data Valid(string value) => new(value);
}

internal static class DataErrors
{
    public const string CodeInvalid = "data-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Data cannot be empty.");
}

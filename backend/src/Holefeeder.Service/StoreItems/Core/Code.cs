using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.StoreItems.Core;

internal sealed record Code
{
    private Code(string value) => Value = value;

    public string Value { get; }

    public static Result<Code> Create(string value) =>
        value.Length switch
        {
            0 or > 100 => CodeErrors.Invalid,
            _ => new Code(value)
        };

    public static Code Valid(string value) => new(value);
}

internal static class CodeErrors
{
    public const string CodeInvalid = $"{nameof(Code)}.{nameof(Invalid)}";

    public static ResultError Invalid => new(CodeInvalid, "Invalid Code");
}

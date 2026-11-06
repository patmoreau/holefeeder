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
    public const string CodeInvalid = "code-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Code must be between 1 and 100 characters.");
}

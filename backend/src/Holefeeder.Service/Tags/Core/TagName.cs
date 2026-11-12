using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Tags.Core;

internal sealed record TagName
{
    private TagName(string value) => Value = value;

    public string Value { get; }

    public static Result<TagName> Create(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Contains(',', StringComparison.Ordinal)
            ? TagNameErrors.Invalid
            : new TagName(value.Trim().ToLowerInvariant());

    public static Result<TagName> Existing(string? value) =>
        string.IsNullOrWhiteSpace(value) ? TagNameErrors.Required : new TagName(value.Trim());

    public static TagName Valid(string value) => new(value);
}

internal static class TagNameErrors
{
    public const string CodeInvalid = "tag-name-invalid";
    public const string CodeRequired = "tag-required";

    public static ResultError Invalid => new(CodeInvalid, "A tag name must not be empty or contain a comma.");

    public static ResultError Required => new(CodeRequired, "A tag is required.");
}

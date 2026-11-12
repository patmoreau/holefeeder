namespace Holefeeder.Service.Categories.Core;

internal sealed record CategoryName
{
    private CategoryName(string value) => Value = value;

    public string Value { get; }

    public static CategoryName Valid(string value) => new(value);
}

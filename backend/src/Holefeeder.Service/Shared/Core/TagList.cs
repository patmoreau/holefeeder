namespace Holefeeder.Service.Shared.Core;

internal sealed record TagList
{
    private TagList(string text) => Text = text;

    public string Text { get; }

    public IReadOnlyList<string> Values => Text.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public static TagList Create(IEnumerable<string?>? values) =>
        new(string.Join(',', (values ?? [])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim().ToLowerInvariant())
            .Distinct()));
}

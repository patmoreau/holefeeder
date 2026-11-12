namespace Holefeeder.Service.Shared.Core;

internal sealed record Tags
{
    private Tags(string text) => Text = text;

    public string Text { get; }

    public IReadOnlyList<string> Values => Text.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public static Tags Create(IEnumerable<string?>? values) =>
        new(string.Join(',', (values ?? [])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim().ToLowerInvariant())
            .Distinct()));
}

namespace Holefeeder.Service.Shared.Core;

internal sealed record FlowTitle(string Title, string? Category)
{
    public static FlowTitle Of(string description, string categoryName)
    {
        var trimmed = description.Trim();

        return trimmed.Length > 0 ? new FlowTitle(trimmed, categoryName) : new FlowTitle(categoryName, null);
    }
}

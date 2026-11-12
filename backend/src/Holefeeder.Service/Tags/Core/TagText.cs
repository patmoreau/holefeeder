namespace Holefeeder.Service.Tags.Core;

internal static class TagText
{
    public static string Rename(string text, TagName tag, TagName newTag) =>
        string.Join(',', Split(text).Select(value => value == tag.Value ? newTag.Value : value).Distinct());

    public static string Remove(string text, TagName tag) =>
        string.Join(',', Split(text).Where(value => value != tag.Value));

    private static string[] Split(string text) =>
        text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}

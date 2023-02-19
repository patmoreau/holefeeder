namespace Holefeeder.Application.Features.Transactions;

internal static class TagsMapper
{
    private const char DELIMITER = ',';

    public static string Map(IEnumerable<string> tagList)
    {
        return string.Join(DELIMITER, tagList);
    }

    public static string[] Map(string tags)
    {
        return string.IsNullOrWhiteSpace(tags) ? Array.Empty<string>() : tags.Split(DELIMITER);
    }
}

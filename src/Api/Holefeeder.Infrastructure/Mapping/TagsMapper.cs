namespace Holefeeder.Infrastructure.Mapping;

public class TagsMapper
{
    private const char DELIMITER = ',';

    public string Map(IEnumerable<string> tagList)
    {
        return string.Join(DELIMITER, tagList);
    }

    public string[] Map(string tags)
    {
        return string.IsNullOrWhiteSpace(tags) ? Array.Empty<string>() : tags.Split(DELIMITER);
    }
}

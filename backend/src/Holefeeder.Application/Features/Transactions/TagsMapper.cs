namespace Holefeeder.Application.Features.Transactions;

internal static class TagsMapper
{
    private const char Delimiter = ',';

    public static string Map(IEnumerable<string> tagList) => string.Join(Delimiter, tagList);
}

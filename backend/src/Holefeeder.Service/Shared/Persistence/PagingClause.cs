using Dapper;

using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.Service.Shared.Persistence;

internal static class PagingClause
{
    public static string Create(Offset offset, Limit limit, DynamicParameters parameters)
    {
        parameters.Add("Offset", offset.Value);
        parameters.Add("Limit", limit.Value);
        return "OFFSET @Offset LIMIT @Limit";
    }
}

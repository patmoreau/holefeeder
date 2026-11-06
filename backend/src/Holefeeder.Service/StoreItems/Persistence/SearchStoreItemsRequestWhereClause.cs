using Dapper;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.StoreItems.Persistence;

internal class SearchStoreItemsRequestWhereClause
{
    private static readonly Dictionary<FilterByOperator, string> Operation = new()
    {
        [FilterByOperator.Equal] = "=",
        [FilterByOperator.NotEqual] = "<>"
    };

    public static string Create(SearchStoreItemsRequestFilterBy request, User user, DynamicParameters parameters)
    {
        var clauses = new List<string>();
        if (request is SearchStoreItemsRequestFilterBy.ByCode code)
        {
            clauses.Add(CreateCodeClause(code.Code, code.Operator, parameters));
        }
        clauses.Add(CreateUserClause(user, parameters));
        var whereClause = string.Join(" AND ", clauses);
        return $"WHERE {whereClause}";
    }

    private static string CreateCodeClause(Code code, FilterByOperator byOperator, DynamicParameters parameters)
    {
        parameters.Add("Code", code.Value);
        return $"s.code {Operation[byOperator]} @Code";
    }

    private static string CreateUserClause(User user, DynamicParameters parameters)
    {
        parameters.Add("UserId", user.Id.Value);
        return "u.id = @UserId";
    }
}

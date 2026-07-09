using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.StoreItem;

public partial record StoreItem
{
    public Result<StoreItem> Modify(string? code = null, string? data = null)
    {
        var newCode = code ?? Code;
        var newData = data ?? Data;

        var result = ResultAggregate.Create()
            .Ensure(CodeValidation(newCode));

        return result.OnSuccess(() => (this with
        {
            Code = newCode,
            Data = newData
        }).ToResult());
    }
}

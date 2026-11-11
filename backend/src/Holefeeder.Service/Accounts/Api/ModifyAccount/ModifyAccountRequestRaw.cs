using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.ModifyAccount;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Api.ModifyAccount;

internal sealed record ModifyAccountRequestRaw([FromBody] ModifyAccountBody Body)
{
    public static Result<ModifyAccountRequest> ToModifyAccountRequest(ModifyAccountRequestRaw raw, User user)
    {
        var body = raw.Body;
        var id = Id.Create(body.Id);
        var name = AccountName.Create(body.Name);
        var type = body.Type is null ? null : (Result<AccountType>?)AccountType.Create(body.Type);
        var openDate = body.OpenDate is null ? null : (Result<DateOnly>?)OpenDate.Parse(body.OpenDate);
        var openBalance = body.OpenBalance is null ? null : (Result<Money>?)Money.Create(body.OpenBalance);

        var validation = ResultAggregate.Create();
        AddFailure(validation, id);
        AddFailure(validation, name);
        AddFailure(validation, type);
        AddFailure(validation, openDate);
        AddFailure(validation, openBalance);

        return validation.OnSuccess<ModifyAccountRequest>(() => new ModifyAccountRequest(
            user, id.Value, name.Value, type?.Value, openDate?.Value, openBalance?.Value, body.Description));
    }

    private static void AddFailure<T>(ResultAggregate validation, Result<T> result)
    {
        if (result.IsFailure)
        {
            validation.AddResult(result.Error);
        }
    }

    private static void AddFailure<T>(ResultAggregate validation, Result<T>? result)
    {
        if (result is { IsFailure: true } failure)
        {
            validation.AddResult(failure.Error);
        }
    }
}

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.OpenAccount;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Api.OpenAccount;

internal sealed record OpenAccountRequestRaw([FromBody] OpenAccountBody Body)
{
    public static Result<OpenAccountRequest> ToOpenAccountRequest(OpenAccountRequestRaw raw, User user)
    {
        var body = raw.Body;
        var type = AccountType.Create(body.Type);
        var name = AccountName.Create(body.Name);
        var openDate = ParseOpenDate(body.OpenDate);
        Result<Money> openBalance = body.OpenBalance is null ? Money.Zero : Money.Create(body.OpenBalance);

        var validation = ResultAggregate.Create();
        AddFailure(validation, type);
        AddFailure(validation, name);
        AddFailure(validation, openDate);
        AddFailure(validation, openBalance);

        return validation.OnSuccess<OpenAccountRequest>(() => new OpenAccountRequest(
            user, type.Value, name.Value, openDate.Value, openBalance.Value, body.Description ?? string.Empty));
    }

    private static Result<DateOnly> ParseOpenDate(string? value)
    {
        var date = IsoDate.Parse(value, OpenAccountRequestErrors.OpenDateInvalid);
        return date.IsSuccess && date.Value == DateOnly.MinValue ? OpenAccountRequestErrors.OpenDateInvalid : date;
    }

    private static void AddFailure<T>(ResultAggregate validation, Result<T> result)
    {
        if (result.IsFailure)
        {
            validation.AddResult(result.Error);
        }
    }
}

internal static class OpenAccountRequestErrors
{
    public const string CodeOpenDateInvalid = "open-date-invalid";

    public static ResultError OpenDateInvalid => new(CodeOpenDateInvalid, "Open date must be a date as YYYY-MM-DD.");
}

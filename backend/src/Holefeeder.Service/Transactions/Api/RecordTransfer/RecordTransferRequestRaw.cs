using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core.RecordTransfer;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Api.RecordTransfer;

internal sealed record RecordTransferRequestRaw([FromBody] RecordTransferBody Body)
{
    public static Result<RecordTransferRequest> ToRecordTransferRequest(RecordTransferRequestRaw raw, User user)
    {
        var body = raw.Body;
        var date = IsoDate.Parse(body.Date, IsoDateErrors.DateInvalid);
        var amount = Money.Create(body.Amount);
        var fromAccountId = ParseId(body.FromAccountId, RecordTransferRequestErrors.FromAccountIdInvalid);
        var toAccountId = ParseId(body.ToAccountId, RecordTransferRequestErrors.ToAccountIdInvalid);

        var validation = ResultAggregate.Create();
        AddFailure(validation, date);
        AddFailure(validation, amount);
        AddFailure(validation, fromAccountId);
        AddFailure(validation, toAccountId);

        return validation.OnSuccess<RecordTransferRequest>(() => new RecordTransferRequest(
            user, date.Value, amount.Value, body.Description ?? string.Empty, fromAccountId.Value, toAccountId.Value));
    }

    private static Result<Id> ParseId(string? value, ResultError error)
    {
        var id = Id.Create(value);
        return id.IsFailure ? error : id;
    }

    private static void AddFailure<T>(ResultAggregate validation, Result<T> result)
    {
        if (result.IsFailure)
        {
            validation.AddResult(result.Error);
        }
    }
}

internal static class RecordTransferRequestErrors
{
    public const string CodeFromAccountIdInvalid = "from-account-id-invalid";
    public const string CodeToAccountIdInvalid = "to-account-id-invalid";

    public static ResultError FromAccountIdInvalid => new(CodeFromAccountIdInvalid, "From account id must be an id.");

    public static ResultError ToAccountIdInvalid => new(CodeToAccountIdInvalid, "To account id must be an id.");
}

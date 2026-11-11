using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.RecordPurchase;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Api.RecordPurchase;

internal sealed record RecordPurchaseRequestRaw([FromBody] RecordPurchaseBody Body)
{
    public static Result<RecordPurchaseRequest> ToRecordPurchaseRequest(RecordPurchaseRequestRaw raw, User user)
    {
        var body = raw.Body;
        var date = IsoDate.Parse(body.Date, IsoDateErrors.DateInvalid);
        var amount = Money.Create(body.Amount);
        var accountId = ParseId(body.AccountId, TransactionRequestErrors.AccountIdInvalid);
        var categoryId = ParseId(body.CategoryId, TransactionRequestErrors.CategoryIdInvalid);

        var validation = ResultAggregate.Create();
        AddFailure(validation, date);
        AddFailure(validation, amount);
        AddFailure(validation, accountId);
        AddFailure(validation, categoryId);

        Repeat? repeat = null;
        if (body.Repeat is { } repeatBody)
        {
            var startDate = IsoDate.Parse(repeatBody.StartDate, RecordPurchaseRequestErrors.RepeatStartDateInvalid);
            var intervalType = IntervalType.Create(repeatBody.IntervalType);
            var frequency = Frequency.Create(repeatBody.Frequency);
            AddFailure(validation, startDate);
            AddFailure(validation, intervalType);
            AddFailure(validation, frequency);
            if (startDate.IsSuccess && intervalType.IsSuccess && frequency.IsSuccess)
            {
                repeat = new Repeat(startDate.Value, intervalType.Value, frequency.Value);
            }
        }

        return validation.OnSuccess<RecordPurchaseRequest>(() => new RecordPurchaseRequest(
            user, date.Value, amount.Value, body.Description ?? string.Empty, accountId.Value, categoryId.Value, Tags.Create(body.Tags),
            repeat));
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

internal static class RecordPurchaseRequestErrors
{
    public const string CodeRepeatStartDateInvalid = "repeat-start-date-invalid";

    public static ResultError RepeatStartDateInvalid => new(CodeRepeatStartDateInvalid, "Repeat start date must be a date as YYYY-MM-DD.");
}

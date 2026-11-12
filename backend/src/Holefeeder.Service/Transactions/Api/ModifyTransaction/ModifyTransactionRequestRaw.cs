using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.ModifyTransaction;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Api.ModifyTransaction;

internal sealed record ModifyTransactionRequestRaw([FromBody] ModifyTransactionBody Body)
{
    public static Result<ModifyTransactionRequest> ToModifyTransactionRequest(ModifyTransactionRequestRaw raw, User user)
    {
        var body = raw.Body;
        var id = Id.Create(body.Id);
        var date = IsoDate.Parse(body.Date, IsoDateErrors.DateInvalid);
        var amount = Money.Create(body.Amount);
        var accountId = ParseId(body.AccountId, TransactionRequestErrors.AccountIdInvalid);
        var categoryId = ParseId(body.CategoryId, TransactionRequestErrors.CategoryIdInvalid);

        var validation = ResultAggregate.Create();
        AddFailure(validation, id);
        AddFailure(validation, date);
        AddFailure(validation, amount);
        AddFailure(validation, accountId);
        AddFailure(validation, categoryId);

        return validation.OnSuccess<ModifyTransactionRequest>(() => new ModifyTransactionRequest(
            user, id.Value, date.Value, amount.Value, body.Description, accountId.Value, categoryId.Value, TagList.Create(body.Tags)));
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

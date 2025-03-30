using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow
{
    private static Func<Result<Nothing>> IdValidation(CashflowId value) =>
        () => value != CashflowId.Empty
            ? Nothing.Value
            : CashflowErrors.IdRequired;

    private static Func<Result<Nothing>> EffectiveDateValidation(DateOnly value) =>
        () => value != default
            ? Nothing.Value
            : CashflowErrors.EffectiveDateRequired;

    private static Func<Result<Nothing>> FrequencyValidation(int value) =>
        () => value > 0
            ? Nothing.Value
            : CashflowErrors.FrequencyInvalid;

    private static Func<Result<Nothing>> RecurrenceValidation(int value) =>
        () => value >= 0
            ? Nothing.Value
            : CashflowErrors.RecurrenceInvalid;

    private static Func<Result<Nothing>> AccountIdValidation(AccountId value) =>
        () => value != AccountId.Empty
            ? Nothing.Value
            : CashflowErrors.AccountIdRequired;

    private static Func<Result<Nothing>> CategoryIdValidation(CategoryId value) =>
        () => value != CategoryId.Empty
            ? Nothing.Value
            : CashflowErrors.CategoryIdRequired;

    private static Func<Result<Nothing>> UserIdValidation(UserId value) =>
        () => value != UserId.Empty
            ? Nothing.Value
            : CashflowErrors.UserIdRequired;
}

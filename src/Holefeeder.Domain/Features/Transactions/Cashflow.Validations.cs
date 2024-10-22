using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow
{
    private static Func<Result<Nothing>> IdValidation(CashflowId value) =>
        () => value != CashflowId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.IdRequired);

    private static Func<Result<Nothing>> EffectiveDateValidation(DateOnly value) =>
        () => value != default
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.EffectiveDateRequired);

    private static Func<Result<Nothing>> FrequencyValidation(int value) =>
        () => value > 0
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.FrequencyInvalid);

    private static Func<Result<Nothing>> RecurrenceValidation(int value) =>
        () => value >= 0
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.RecurrenceInvalid);

    private static Func<Result<Nothing>> AccountIdValidation(AccountId value) =>
        () => value != AccountId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.AccountIdRequired);

    private static Func<Result<Nothing>> CategoryIdValidation(CategoryId value) =>
        () => value != CategoryId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.CategoryIdRequired);

    private static Func<Result<Nothing>> UserIdValidation(UserId value) =>
        () => value != UserId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CashflowErrors.UserIdRequired);
}

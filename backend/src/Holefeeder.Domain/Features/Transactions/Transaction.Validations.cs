using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Transaction
{
    private static Func<Result<Nothing>> IdValidation(TransactionId value) =>
        () => value != TransactionId.Empty
            ? Nothing.Value
            : TransactionErrors.IdRequired;

    private static Func<Result<Nothing>> DateValidation(DateOnly value) =>
        () => value != default
            ? Nothing.Value
            : TransactionErrors.DateRequired;

    private static Func<Result<Nothing>> FrequencyValidation(int value) =>
        () => value > 0
            ? Nothing.Value
            : TransactionErrors.FrequencyInvalid;

    private static Func<Result<Nothing>> RecurrenceValidation(int value) =>
        () => value >= 0
            ? Nothing.Value
            : TransactionErrors.RecurrenceInvalid;

    private static Func<Result<Nothing>> AccountIdValidation(AccountId value) =>
        () => value != AccountId.Empty
            ? Nothing.Value
            : TransactionErrors.AccountIdRequired;

    private static Func<Result<Nothing>> CategoryIdValidation(CategoryId value) =>
        () => value != CategoryId.Empty
            ? Nothing.Value
            : TransactionErrors.CategoryIdRequired;

    private static Func<Result<Nothing>> UserIdValidation(UserId value) =>
        () => value != UserId.Empty
            ? Nothing.Value
            : TransactionErrors.UserIdRequired;

    private static Func<Result<Nothing>> CashflowValidation(CashflowId value, DateOnly dateValue) =>
        () => value != CashflowId.Empty && dateValue != default
            ? Nothing.Value
            : TransactionErrors.CashflowRequired;
}

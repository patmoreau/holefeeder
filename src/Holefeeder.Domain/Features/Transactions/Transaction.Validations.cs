using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Transaction
{
    private static Func<Result<Nothing>> IdValidation(TransactionId value) =>
        () => value != TransactionId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.IdRequired);

    private static Func<Result<Nothing>> DateValidation(DateOnly value) =>
        () => value != default
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.DateRequired);

    private static Func<Result<Nothing>> FrequencyValidation(int value) =>
        () => value > 0
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.FrequencyInvalid);

    private static Func<Result<Nothing>> RecurrenceValidation(int value) =>
        () => value >= 0
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.RecurrenceInvalid);

    private static Func<Result<Nothing>> AccountIdValidation(AccountId value) =>
        () => value != AccountId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.AccountIdRequired);

    private static Func<Result<Nothing>> CategoryIdValidation(CategoryId value) =>
        () => value != CategoryId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.CategoryIdRequired);

    private static Func<Result<Nothing>> UserIdValidation(UserId value) =>
        () => value != UserId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.UserIdRequired);

    private static Func<Result<Nothing>> CashflowValidation(CashflowId value, DateOnly dateValue) =>
        () => value != CashflowId.Empty && dateValue != default
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(TransactionErrors.CashflowRequired);
}

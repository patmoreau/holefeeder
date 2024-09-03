using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Transaction
{
    private static ResultValidation IdValidation(TransactionId value) =>
        ResultValidation.Create(() => value != TransactionId.Empty, TransactionErrors.IdRequired);

    private static ResultValidation DateValidation(DateOnly value) =>
        ResultValidation.Create(() => value != default, TransactionErrors.DateRequired);

    private static ResultValidation FrequencyValidation(int value) =>
        ResultValidation.Create(() => value > 0, TransactionErrors.FrequencyInvalid);

    private static ResultValidation RecurrenceValidation(int value) =>
        ResultValidation.Create(() => value >= 0, TransactionErrors.RecurrenceInvalid);

    private static ResultValidation AccountIdValidation(AccountId value) =>
        ResultValidation.Create(() => value != AccountId.Empty, TransactionErrors.AccountIdRequired);

    private static ResultValidation CategoryIdValidation(CategoryId value) =>
        ResultValidation.Create(() => value != CategoryId.Empty, TransactionErrors.CategoryIdRequired);

    private static ResultValidation UserIdValidation(UserId value) =>
        ResultValidation.Create(() => value != UserId.Empty, TransactionErrors.UserIdRequired);

    private static ResultValidation CashflowValidation(CashflowId value, DateOnly dateValue) =>
        ResultValidation.Create(() => value != CashflowId.Empty && dateValue != default,
            TransactionErrors.CashflowRequired);
}

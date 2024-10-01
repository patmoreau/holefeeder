using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow
{
    private static ResultValidation IdValidation(CashflowId value) =>
        ResultValidation.Create(() => value != CashflowId.Empty, CashflowErrors.IdRequired);

    private static ResultValidation EffectiveDateValidation(DateOnly value) =>
        ResultValidation.Create(() => value != default, CashflowErrors.EffectiveDateRequired);

    private static ResultValidation FrequencyValidation(int value) =>
        ResultValidation.Create(() => value > 0, CashflowErrors.FrequencyInvalid);

    private static ResultValidation RecurrenceValidation(int value) =>
        ResultValidation.Create(() => value >= 0, CashflowErrors.RecurrenceInvalid);

    private static ResultValidation AccountIdValidation(AccountId value) =>
        ResultValidation.Create(() => value != AccountId.Empty, CashflowErrors.AccountIdRequired);

    private static ResultValidation CategoryIdValidation(CategoryId value) =>
        ResultValidation.Create(() => value != CategoryId.Empty, CashflowErrors.CategoryIdRequired);

    private static ResultValidation UserIdValidation(UserId value) =>
        ResultValidation.Create(() => value != UserId.Empty, CashflowErrors.UserIdRequired);
}

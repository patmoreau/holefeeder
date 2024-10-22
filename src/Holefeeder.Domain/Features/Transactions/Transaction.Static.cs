using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Transaction
{
    public static Result<Transaction> Import(TransactionId id, DateOnly date, Money amount, string description,
        AccountId accountId, CategoryId categoryId, CashflowId? cashflowId, DateOnly? cashflowDate, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(IdValidation(id))
            .Ensure(DateValidation(date))
            .Ensure(AccountIdValidation(accountId))
            .Ensure(CategoryIdValidation(categoryId))
            .Ensure(UserIdValidation(userId));

        return result.Switch(
            () => Result<Transaction>.Success(new Transaction(id, date, amount, accountId, categoryId, userId)
            {
                Amount = amount,
                Description = description,
                CashflowId = cashflowId,
                CashflowDate = cashflowDate
            }),
            Result<Transaction>.Failure);
    }

    public static Result<Transaction> Create(DateOnly date, Money amount, string description, AccountId accountId,
        CategoryId categoryId, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(DateValidation(date))
            .Ensure(AccountIdValidation(accountId))
            .Ensure(CategoryIdValidation(categoryId))
            .Ensure(UserIdValidation(userId));

        return result.Switch(
            () => Result<Transaction>.Success(new Transaction(TransactionId.New, date, amount, accountId, categoryId, userId)
            {
                Amount = amount,
                UserId = userId,
                Description = description
            }),
            Result<Transaction>.Failure);
    }
}

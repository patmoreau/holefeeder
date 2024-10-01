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
        var result = Result.Validate(IdValidation(id), DateValidation(date), AccountIdValidation(accountId),
            CategoryIdValidation(categoryId), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Transaction>.Failure(result.Error);
        }

        return Result<Transaction>.Success(new Transaction(id, date, amount, accountId, categoryId, userId)
        {
            Amount = amount,
            Description = description,
            CashflowId = cashflowId,
            CashflowDate = cashflowDate
        });
    }

    public static Result<Transaction> Create(DateOnly date, Money amount, string description, AccountId accountId,
        CategoryId categoryId, UserId userId)
    {
        var result = Result.Validate(DateValidation(date), AccountIdValidation(accountId),
            CategoryIdValidation(categoryId), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Transaction>.Failure(result.Error);
        }

        return Result<Transaction>.Success(
            new Transaction(TransactionId.New, date, amount, accountId, categoryId, userId)
            {
                Amount = amount,
                UserId = userId,
                Description = description
            });
    }
}

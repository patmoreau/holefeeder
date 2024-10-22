using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Transaction
{
    public Result<Transaction> SetTags(params string[] tags)
    {
        var newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().Select(x => x.ToLowerInvariant())
            .ToList();

        _tags = newTags.ToImmutableArray();
        return Result<Transaction>.Success(this);
    }

    public Result<Transaction> ApplyCashflow(CashflowId cashflowId, DateOnly cashflowDate)
    {
        var result = ResultAggregate.Create()
            .Ensure(CashflowValidation(cashflowId, cashflowDate));

        return result.Switch(
            () => Result<Transaction>.Success(this with
            {
                CashflowId = cashflowId,
                CashflowDate = cashflowDate
            }),
            Result<Transaction>.Failure);
    }

    public Result<Transaction> Modify(DateOnly? date = null, Money? amount = null, string? description = null,
        AccountId? accountId = null, CategoryId? categoryId = null, CashflowId? cashflowId = null,
        DateOnly? cashflowDate = null)
    {
        var newDate = date ?? Date;
        var newAmount = amount ?? Amount;
        var newDescription = description ?? Description;
        var newAccountId = accountId ?? AccountId;
        var newCategoryId = categoryId ?? CategoryId;
        var newCashflowId = cashflowId ?? CashflowId;
        var newCashflowDate = cashflowDate ?? CashflowDate;

        var result = ResultAggregate.Create()
            .Ensure(DateValidation(newDate))
            .Ensure(AccountIdValidation(newAccountId))
            .Ensure(CategoryIdValidation(newCategoryId));

        return result.Switch(
            () => Result<Transaction>.Success(
                this with
                {
                    Date = newDate,
                    Amount = newAmount,
                    Description = newDescription,
                    AccountId = newAccountId,
                    CategoryId = newCategoryId,
                    CashflowId = newCashflowId,
                    CashflowDate = newCashflowDate
                }),
            Result<Transaction>.Failure);
    }
}

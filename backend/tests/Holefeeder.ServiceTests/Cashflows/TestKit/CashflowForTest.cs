using Dapper;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Cashflows.TestKit;

internal static class CashflowForTest
{
    private const string InsertAccount =
        """
        INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id)
        VALUES (@Id, 'Checking', @Name, FALSE, 0, '2020-01-01', '', FALSE, @UserId);
        """;

    private const string InsertCategory =
        """
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, user_id)
        VALUES (@Id, 'Expense', @Name, '#000000', 0, FALSE, FALSE, @UserId);
        """;

    private const string InsertCashflow =
        """
        INSERT INTO cashflows (id, effective_date, amount, interval_type, frequency, recurrence, description, account_id, category_id, inactive, tags, user_id)
        VALUES (@Id, @EffectiveDate, @Amount, @IntervalType, @Frequency, @Recurrence, @Description, @AccountId, @CategoryId, @Inactive, @Tags, @UserId);
        """;

    private const string SelectCashflow =
        """
        SELECT id, effective_date, amount, interval_type, frequency, recurrence, description, account_id, category_id, inactive, tags
        FROM cashflows
        WHERE id = @Id;
        """;

    private const string SelectPayments =
        """
        SELECT id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags
        FROM transactions
        WHERE cashflow_id = @CashflowId
        ORDER BY date;
        """;

    internal static Cashflow ACashflow(
        decimal amount = 1450m, bool inactive = false, string description = "Rent", string tags = "housing", Id? accountId = null, Id? categoryId = null) =>
        Cashflow.Valid(
            AnId(), new DateOnly(2026, 1, 1), Money.Valid(amount), "Monthly", 1, 0, description,
            accountId ?? AnId(), categoryId ?? AnId(), inactive, tags);

    public static async Task<Cashflow> SavedInDbAsync(this Cashflow cashflow, User user, DatabaseForTest database)
    {
        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(InsertAccount, new { Id = cashflow.AccountId.Value, Name = $"account-{cashflow.AccountId.Value}", UserId = user.Id.Value });
        await connection.ExecuteAsync(InsertCategory, new { Id = cashflow.CategoryId.Value, Name = $"category-{cashflow.CategoryId.Value}", UserId = user.Id.Value });
        await connection.ExecuteAsync(InsertCashflow, new
        {
            Id = cashflow.Id.Value,
            EffectiveDate = cashflow.EffectiveDate.ToDateTime(TimeOnly.MinValue),
            Amount = cashflow.Amount.Value,
            cashflow.IntervalType,
            cashflow.Frequency,
            cashflow.Recurrence,
            cashflow.Description,
            AccountId = cashflow.AccountId.Value,
            CategoryId = cashflow.CategoryId.Value,
            cashflow.Inactive,
            cashflow.Tags,
            UserId = user.Id.Value
        });
        return cashflow;
    }

    public static async Task<Cashflow?> CashflowInDbAsync(this DatabaseForTest database, Id id)
    {
        await using var connection = database.DataSource.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<StoredCashflow>(SelectCashflow, new { Id = id.Value });
        return row is null
            ? null
            : Cashflow.Valid(
                Id.Valid(row.Id), row.EffectiveDate, Money.Valid(row.Amount), row.IntervalType, row.Frequency, row.Recurrence,
                row.Description ?? string.Empty, Id.Valid(row.AccountId), Id.Valid(row.CategoryId), row.Inactive, row.Tags ?? string.Empty);
    }

    public static async Task<IReadOnlyList<Payment>> PaymentsInDbAsync(this DatabaseForTest database, Id cashflowId)
    {
        await using var connection = database.DataSource.CreateConnection();
        var rows = await connection.QueryAsync<StoredPayment>(SelectPayments, new { CashflowId = cashflowId.Value });
        return
        [
            .. rows.Select(row => new Payment(
                Id.Valid(row.Id), row.Date, Money.Valid(row.Amount), row.Description ?? string.Empty, Id.Valid(row.AccountId),
                Id.Valid(row.CategoryId), Id.Valid(row.CashflowId), row.CashflowDate, row.Tags ?? string.Empty))
        ];
    }

    private sealed record StoredCashflow(
        Guid Id, DateOnly EffectiveDate, decimal Amount, string IntervalType, int Frequency, int Recurrence, string? Description,
        Guid AccountId, Guid CategoryId, bool Inactive, string? Tags);

    private sealed record StoredPayment(
        Guid Id, DateOnly Date, decimal Amount, string? Description, Guid AccountId, Guid CategoryId, Guid CashflowId,
        DateOnly CashflowDate, string? Tags);
}

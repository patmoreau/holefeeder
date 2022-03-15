using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders;

public class TransactionBuilder
{
    private static readonly object _locker = new();
    private static int _seed = 1;
    private static readonly List<TransactionEntity> _transactions = new();
    private Guid _account;
    private decimal _amount;
    private Guid _cashflow;
    private DateTime? _cashflowDate;
    private Guid _category;
    private DateTime _date;
    private readonly string _description;

    private readonly Guid _id;
    private Guid _userId;

    private TransactionBuilder(Guid id, int seed)
    {
        _id = id;
        _amount = (Convert.ToDecimal(seed) * 100m) + (Convert.ToDecimal(seed) / 100m);
        _date = new DateTime(2020, 1, 1).AddDays(seed);
        _description = $"Transaction{seed}";
        _account = Guid.Empty;
        _category = Guid.Empty;
        _userId = Guid.Empty;
        _cashflow = Guid.Empty;
        _cashflowDate = null;
    }

    public static IReadOnlyList<TransactionEntity> Transactions => _transactions;

    public static TransactionBuilder Create(Guid id)
    {
        lock (_locker)
        {
            return new TransactionBuilder(id, _seed);
        }
    }

    public TransactionBuilder OfAmount(decimal amount)
    {
        _amount = amount;

        return this;
    }

    public TransactionBuilder On(DateTime date)
    {
        _date = date;

        return this;
    }

    public TransactionBuilder ForAccount(Guid account)
    {
        _account = account;

        return this;
    }

    public TransactionBuilder ForCategory(Guid category)
    {
        _category = category;

        return this;
    }

    public TransactionBuilder IsCashflow(Guid cashflow, DateTime cashflowDate)
    {
        _cashflow = cashflow;
        _cashflowDate = cashflowDate;

        return this;
    }

    public TransactionBuilder ForUser(Guid userId)
    {
        _userId = userId;

        return this;
    }

    public void Build()
    {
        var schema = new TransactionEntity
        {
            Id = _id,
            Amount = _amount,
            Date = _date,
            Description = _description,
            UserId = _userId,
            AccountId = _account,
            CategoryId = _category,
            CashflowId = _cashflow == Guid.Empty ? null : _cashflow,
            CashflowDate = _cashflowDate
        };

        lock (_locker)
        {
            _seed++;
            _transactions.Add(schema);
        }
    }
}

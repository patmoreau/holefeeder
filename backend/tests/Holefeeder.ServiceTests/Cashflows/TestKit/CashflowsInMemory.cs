using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Cashflows.TestKit;

internal sealed class CashflowsInMemory : ICashflows
{
    private readonly List<(Id UserId, Cashflow Cashflow)> _cashflows = [];
    private readonly List<(Id UserId, Payment Payment)> _payments = [];
    private ResultError? _failure;

    public IReadOnlyList<Payment> PaymentsOf(User user) =>
        [.. _payments.Where(entry => entry.UserId == user.Id).Select(entry => entry.Payment)];

    public Cashflow? CashflowOf(User user, Id id) =>
        _cashflows.Where(entry => entry.UserId == user.Id && entry.Cashflow.Id == id).Select(entry => entry.Cashflow).SingleOrDefault();

    public Task<Result<Cashflow>> GetAsync(User user, Id id, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Cashflow>>(_failure);
        }

        var cashflow = CashflowOf(user, id);
        return Task.FromResult<Result<Cashflow>>(cashflow is null ? CashflowErrors.NotFound : cashflow);
    }

    public Task<Result<Nothing>> SaveAsync(User user, Cashflow cashflow, Payment? payment, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Nothing>>(_failure);
        }

        var index = _cashflows.FindIndex(entry => entry.Cashflow.Id == cashflow.Id);
        if (index >= 0 && _cashflows[index].UserId != user.Id)
        {
            return Task.FromResult<Result<Nothing>>(CashflowErrors.NotFound);
        }

        if (index < 0)
        {
            _cashflows.Add((user.Id, cashflow));
        }
        else
        {
            _cashflows[index] = (user.Id, cashflow);
        }

        if (payment is not null)
        {
            _payments.Add((user.Id, payment));
        }

        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    public void Add(User user, Cashflow cashflow) => _cashflows.Add((user.Id, cashflow));

    public void FailWith(ResultError error) => _failure = error;
}

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core.ListCashflows;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;

namespace Holefeeder.ServiceTests.Cashflows.Core.ListCashflows;

public class ListCashflowsUseCaseTests
{
    private readonly CashflowsInMemory _cashflows = new();
    private readonly ListCashflowsUseCase _useCase;

    public ListCashflowsUseCaseTests() => _useCase = new ListCashflowsUseCase(_cashflows);

    [Fact]
    public async Task HandleAsync_ListsTheUsersCashflows()
    {
        var user = AUser();
        var rent = ACashflowSummary("Rent");
        var payroll = ACashflowSummary("Payroll", "Salary", CategoryType.Gain, 2400m);
        _cashflows.Add(user, rent);
        _cashflows.Add(user, payroll);
        _cashflows.Add(AUser(), ACashflowSummary("Someone else's"));

        var result = await _useCase.HandleAsync(new ListCashflowsRequest(user), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(rent, payroll);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenCashflowsCannotBeRead()
    {
        var failure = new ResultError("cashflows-unavailable", "Cashflows are unavailable.");
        _cashflows.FailWith(failure);

        var result = await _useCase.HandleAsync(new ListCashflowsRequest(AUser()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}

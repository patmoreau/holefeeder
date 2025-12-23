using Holefeeder.Application.Features.Accounts;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;

namespace Holefeeder.UnitTests.Application.Features.Accounts;

[UnitTest]
public class AccountMapperTests
{
    [Fact]
    public void GivenCalculateUpcomingVariation_WhenCalculated_ThenReturnCorrectProjection()
    {
        var gainCategory = CategoryBuilder.GivenACategory().OfType(CategoryType.Gain).Build();
        var expenseCategory = CategoryBuilder.GivenACategory().OfType(CategoryType.Expense).Build();
        var gainCashflow = CashflowBuilder
            .GivenAnActiveCashflow()
            .ForCategory(gainCategory, true)
            .OnEffectiveDate(new DateOnly(2025, 12, 05))
            .OfFrequency(DateIntervalType.Monthly)
            .OfAmount(1.23m)
            .Build();
        var expenseCashflow = CashflowBuilder
            .GivenAnActiveCashflow()
            .ForCategory(expenseCategory, true)
            .OnEffectiveDate(new DateOnly(2025, 12, 01))
            .OfFrequency(DateIntervalType.Weekly, 2)
            .OfAmount(12.34m)
            .Build();

        var account = AccountBuilder.GivenAnActiveAccount()
            .WithOpenBalance(200)
            .WithCashflows([gainCashflow, expenseCashflow])
            .Build();

        var model = AccountMapper.MapToAccountViewModel(account, new DateOnly(2025, 12, 31), [gainCashflow, expenseCashflow]);

        model.UpcomingVariation.Should().Be(-35.79m);
        model.ProjectedBalance.Should().Be(164.21m);
    }

}

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Tests.Common.SeedWork;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataTransactionDtoBuilder : RootBuilder<MyDataTransactionDto>
{
    protected override Faker<MyDataTransactionDto> Faker { get; } = new AutoFaker<MyDataTransactionDto>()
        .RuleFor(f => f.CashflowId, (Guid?)null)
        .RuleFor(f => f.CashflowDate, (DateTime?)null);

    public static MyDataTransactionDtoBuilder GivenMyTransactionData() => new();

    public MyDataTransactionDtoBuilder WithAccount(MyDataAccountDto account)
    {
        Faker.RuleFor(f => f.AccountId, account.Id);
        return this;
    }

    public MyDataTransactionDtoBuilder WithCategory(MyDataCategoryDto category)
    {
        Faker.RuleFor(f => f.CategoryId, category.Id);
        return this;
    }
}

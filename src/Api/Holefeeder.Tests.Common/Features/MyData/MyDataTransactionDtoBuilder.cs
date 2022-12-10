using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Tests.Common.Builders;

namespace Holefeeder.Tests.Common.Features.MyData;

internal sealed class MyDataTransactionDtoBuilder : IBuilder<MyDataTransactionDto>,
    ICollectionBuilder<MyDataTransactionDto>
{
    private readonly Faker<MyDataTransactionDto> _faker = new AutoFaker<MyDataTransactionDto>()
        .RuleFor(f => f.CashflowId, (Guid?)null)
        .RuleFor(f => f.CashflowDate, (DateTime?)null);

    public static MyDataTransactionDtoBuilder GivenMyTransactionData() => new();

    public MyDataTransactionDtoBuilder WithAccount(MyDataAccountDto account)
    {
        _faker.RuleFor(f => f.AccountId, account.Id);
        return this;
    }

    public MyDataTransactionDtoBuilder WithCategory(MyDataCategoryDto category)
    {
        _faker.RuleFor(f => f.CategoryId, category.Id);
        return this;
    }

    public MyDataTransactionDto Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public MyDataTransactionDto[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}

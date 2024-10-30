using DrifterApps.Seeds.Testing;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataTransactionDtoBuilder : FakerBuilder<MyDataTransactionDto>
{
    protected override Faker<MyDataTransactionDto> Faker { get; } = CreateFaker<MyDataTransactionDto>()
        .RuleFor(f => f.CashflowId, (Guid?)null)
        .RuleFor(f => f.CashflowDate, (DateOnly?)null)
        .RuleFor(f => f.Id, faker => faker.RandomGuid())
        .RuleFor(f => f.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(f => f.Amount, faker => faker.Finance.Amount())
        .RuleFor(f => f.Description, faker => faker.Lorem.Sentence())
        .RuleFor(f => f.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray())
        .RuleFor(f => f.CategoryId, faker => faker.RandomGuid())
        .RuleFor(f => f.AccountId, faker => faker.RandomGuid())
        ;

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

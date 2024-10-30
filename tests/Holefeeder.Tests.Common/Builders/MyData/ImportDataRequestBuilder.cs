using DrifterApps.Seeds.Testing;

using Holefeeder.Application.Features.MyData.Models;

using static Holefeeder.Application.Features.MyData.Commands.ImportData;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal class ImportDataRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateFaker<Request>()
        .RuleFor(x => x.UpdateExisting, false);

    private readonly Faker<Request.Dto> _dtoFaker = new();

    public new Request Build()
    {
        var data = _dtoFaker.Generate();

        Faker.RuleFor(f => f.Data, data);

        return base.Build();
    }

    public static ImportDataRequestBuilder GivenAnImportDataRequest() => new();

    public ImportDataRequestBuilder WithNoData()
    {
        _dtoFaker.RuleFor(f => f.Accounts, []);
        _dtoFaker.RuleFor(f => f.Cashflows, []);
        _dtoFaker.RuleFor(f => f.Categories, []);
        _dtoFaker.RuleFor(f => f.Transactions, []);
        return this;
    }

    public ImportDataRequestBuilder WithUpdateExisting()
    {
        Faker.RuleFor(f => f.UpdateExisting, true);
        return this;
    }

    public ImportDataRequestBuilder WithAccounts(params MyDataAccountDto[] accounts)
    {
        _dtoFaker.RuleFor(f => f.Accounts, accounts);
        return this;
    }

    public ImportDataRequestBuilder WithCashflows(params MyDataCashflowDto[] cashflows)
    {
        _dtoFaker.RuleFor(f => f.Cashflows, cashflows);
        return this;
    }

    public ImportDataRequestBuilder WithCategories(params MyDataCategoryDto[] categories)
    {
        _dtoFaker.RuleFor(f => f.Categories, categories);
        return this;
    }

    public ImportDataRequestBuilder WithTransactions(params MyDataTransactionDto[] transactions)
    {
        _dtoFaker.RuleFor(f => f.Transactions, transactions);
        return this;
    }
}

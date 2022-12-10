using Holefeeder.Application.Features.MyData.Commands.ImportData;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Tests.Common.Builders;

namespace Holefeeder.Tests.Common.Features.MyData;

internal class ImportDataRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();
    private readonly Faker<Request.Dto> _dtoFaker = new AutoFaker<Request.Dto>();

    private ImportDataRequestBuilder()
    {
    }

    public static ImportDataRequestBuilder GivenAnImportDataRequest() => new();

    public ImportDataRequestBuilder WithNoData()
    {
        _dtoFaker.RuleFor(f => f.Accounts, Array.Empty<MyDataAccountDto>());
        _dtoFaker.RuleFor(f => f.Cashflows, Array.Empty<MyDataCashflowDto>());
        _dtoFaker.RuleFor(f => f.Categories, Array.Empty<MyDataCategoryDto>());
        _dtoFaker.RuleFor(f => f.Transactions, Array.Empty<MyDataTransactionDto>());
        return this;
    }

    public ImportDataRequestBuilder WithUpdateExisting()
    {
        _faker.RuleFor(f => f.UpdateExisting, true);
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

    public Request Build()
    {
        var data = _dtoFaker.Generate();

        _faker.RuleFor(f => f.Data, data);

        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }
}

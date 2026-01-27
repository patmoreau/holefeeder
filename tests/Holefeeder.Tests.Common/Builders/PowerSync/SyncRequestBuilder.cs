using DrifterApps.Seeds.Testing;


using static Holefeeder.Application.UseCases.PowerSync;

namespace Holefeeder.Tests.Common.Builders.PowerSync;

internal class SyncRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateFaker<Request>()
        .RuleFor(x => x.TransactionId, faker => faker.Random.Long(min: 1, max: 1000));

    private readonly Faker<SyncOperation> _operationFaker = CreateFaker<SyncOperation>()
        .RuleFor(x => x.Type, faker => faker.PickRandom("accounts", "cashflows", "categories", "store_items", "transactions"))
        .RuleFor(x => x.Op, faker => faker.PickRandom("PUT", "PATCH", "DELETE"))
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Data, () => new Dictionary<string, object>());

    public new Request Build()
    {
        Faker.RuleFor(f => f.Operations, () => [_operationFaker.Generate()]);

        return base.Build();
    }

    public SyncRequestBuilder WithNoTransactionId()
    {
        Faker.RuleFor(f => f.TransactionId, 0);
        return this;
    }

    public SyncRequestBuilder WithType(string table)
    {
        _operationFaker.RuleFor(f => f.Type, table);
        return this;
    }

    public SyncRequestBuilder WithInvalidType()
    {
        _operationFaker.RuleFor(f => f.Type, faker => faker.Lorem.Word());
        return this;
    }

    public SyncRequestBuilder WithId(Guid id)
    {
        _operationFaker.RuleFor(f => f.Id, id);
        return this;
    }

    public SyncRequestBuilder WithNoId()
    {
        _operationFaker.RuleFor(f => f.Id, Guid.Empty);
        return this;
    }

    public SyncRequestBuilder WithInvalidOperation()
    {
        _operationFaker.RuleFor(f => f.Op, faker => faker.Lorem.Word());
        return this;
    }

    public SyncRequestBuilder WithOperation(string operation)
    {
        _operationFaker.RuleFor(f => f.Op, operation);
        return this;
    }

    public SyncRequestBuilder WithData(Dictionary<string, object> data)
    {
        _operationFaker.RuleFor(f => f.Data, data);
        return this;
    }

    public static SyncRequestBuilder GivenASyncRequest() => new();
}

using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class GetUpcomingRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker<Request>()
        .RuleFor(fake => fake.From, fake => fake.Date.PastDateOnly())
        .RuleFor(fake => fake.To, fake => fake.Date.FutureDateOnly())
        .RuleFor(fake => fake.AccountId, () => AccountId.Empty);

    public GetUpcomingRequestBuilder From(DateOnly from)
    {
        Faker.RuleFor(x => x.From, from);
        return this;
    }

    public GetUpcomingRequestBuilder To(DateOnly to)
    {
        Faker.RuleFor(x => x.To, to);
        return this;
    }

    public GetUpcomingRequestBuilder ForAccountId(AccountId accountId)
    {
        Faker.RuleFor(x => x.AccountId, accountId);
        return this;
    }

    public static GetUpcomingRequestBuilder GivenAnUpcomingRequest() => new();

    public static GetUpcomingRequestBuilder GivenAnInvalidUpcomingRequest()
    {
        var builder = new GetUpcomingRequestBuilder();
        builder.Faker
            .RuleFor(fake => fake.To, fake => fake.Date.PastDateOnly())
            .RuleFor(fake => fake.From, fake => fake.Date.FutureDateOnly())
            .RuleFor(fake => fake.AccountId, () => AccountId.Empty);
        return builder;
    }
}

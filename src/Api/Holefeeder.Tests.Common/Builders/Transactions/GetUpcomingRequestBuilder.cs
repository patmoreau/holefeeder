using DrifterApps.Seeds.Testing;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class GetUpcomingRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Date.PastDateOnly(), faker.Date.FutureDateOnly()))
        .RuleFor(fake => fake.From, fake => fake.Date.PastDateOnly())
        .RuleFor(fake => fake.To, fake => fake.Date.FutureDateOnly());

    public GetUpcomingRequestBuilder From(DateOnly from)
    {
        FakerRules.RuleFor(x => x.From, from);
        return this;
    }

    public GetUpcomingRequestBuilder To(DateOnly to)
    {
        FakerRules.RuleFor(x => x.To, to);
        return this;
    }

    public static GetUpcomingRequestBuilder GivenAnUpcomingRequest() => new();

    public static GetUpcomingRequestBuilder GivenAnInvalidUpcomingRequest()
    {
        GetUpcomingRequestBuilder builder = new();
        builder.FakerRules
            .RuleFor(fake => fake.To, fake => fake.Date.PastDateOnly())
            .RuleFor(fake => fake.From, fake => fake.Date.FutureDateOnly());
        return builder;
    }
}

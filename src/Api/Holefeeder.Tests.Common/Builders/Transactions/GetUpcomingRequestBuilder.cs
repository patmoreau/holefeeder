using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class GetUpcomingRequestBuilder : RootBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new AutoFaker<Request>()
        .RuleFor(fake => fake.From, fake => fake.Date.Past().Date)
        .RuleFor(fake => fake.To, fake => fake.Date.Future().Date);

    public GetUpcomingRequestBuilder From(DateTime from)
    {
        Faker.RuleFor(x => x.From, from);
        return this;
    }

    public GetUpcomingRequestBuilder To(DateTime to)
    {
        Faker.RuleFor(x => x.To, to);
        return this;
    }

    public static GetUpcomingRequestBuilder GivenAnUpcomingRequest() => new();

    public static GetUpcomingRequestBuilder GivenAnInvalidUpcomingRequest()
    {
        GetUpcomingRequestBuilder builder = new();
        builder.Faker
            .RuleFor(fake => fake.To, fake => fake.Date.Past().Date)
            .RuleFor(fake => fake.From, fake => fake.Date.Future().Date);
        return builder;
    }
}

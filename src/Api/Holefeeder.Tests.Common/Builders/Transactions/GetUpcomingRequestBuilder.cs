using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class GetUpcomingRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>()
        .RuleFor(fake => fake.From, fake => fake.Date.Past().Date)
        .RuleFor(fake => fake.To, fake => fake.Date.Future().Date);

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public GetUpcomingRequestBuilder From(DateTime from)
    {
        _faker.RuleFor(x => x.From, from);
        return this;
    }

    public GetUpcomingRequestBuilder To(DateTime to)
    {
        _faker.RuleFor(x => x.To, to);
        return this;
    }

    public static GetUpcomingRequestBuilder GivenAnUpcomingRequest() => new();

    public static GetUpcomingRequestBuilder GivenAnInvalidUpcomingRequest()
    {
        GetUpcomingRequestBuilder builder = new GetUpcomingRequestBuilder();
        builder._faker
            .RuleFor(fake => fake.To, fake => fake.Date.Past().Date)
            .RuleFor(fake => fake.From, fake => fake.Date.Future().Date);
        return builder;
    }
}

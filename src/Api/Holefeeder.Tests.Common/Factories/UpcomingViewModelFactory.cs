using System.Collections.Immutable;
using Holefeeder.Application.Models;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class UpcomingViewModelFactory : AutoFaker<UpcomingViewModel>
{
    public UpcomingViewModelFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Date, faker => faker.Date.Past().Date);
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToImmutableArray());
    }
}

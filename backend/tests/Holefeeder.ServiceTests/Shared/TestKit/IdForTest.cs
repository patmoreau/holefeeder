using Bogus;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Shared.TestKit;

internal static class IdForTest
{
    private static readonly Faker Faker = new();

    public static Id AnId() => Id.Valid(Faker.Random.Guid());
}

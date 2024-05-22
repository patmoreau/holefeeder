namespace Holefeeder.Tests.Common.Extensions;

public static class FakerExtensions
{
    private const bool DotNetGeneratedGuid = true;

    public static Guid RandomGuid(this Faker faker)
    {
        ArgumentNullException.ThrowIfNull(faker);

#pragma warning disable CS0162 // Unreachable code detected
        return DotNetGeneratedGuid ? Guid.NewGuid() : faker.Random.Guid();
#pragma warning restore CS0162 // Unreachable code detected
    }
}

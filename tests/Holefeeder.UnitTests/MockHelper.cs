using DrifterApps.Seeds.Application;

using Holefeeder.Tests.Common.Extensions;

using Microsoft.Extensions.Logging;

namespace Holefeeder.UnitTests;

public static class MockHelper
{
    public static ILogger<T> CreateLogger<T>() => Substitute.For<ILogger<T>>();

    public static IUserContext CreateUserContext()
    {
        var userContextMock = Substitute.For<IUserContext>();
        userContextMock.Id.Returns(Fakerizer.RandomGuid());
        return userContextMock;
    }
}

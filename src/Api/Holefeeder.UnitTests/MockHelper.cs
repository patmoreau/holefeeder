using System;

using AutoBogus;

using Holefeeder.Application.SeedWork;

using Microsoft.Extensions.Logging;

using NSubstitute;

namespace Holefeeder.UnitTests;

public static class MockHelper
{
    public static ILogger<T> CreateLogger<T>() => Substitute.For<ILogger<T>>();

    public static IUserContext CreateUserContext()
    {
        var userContextMock = Substitute.For<IUserContext>();
        userContextMock.UserId.Returns(AutoFaker.Generate<Guid>());
        return userContextMock;
    }
}

using System;
using System.Security.Claims;

using AutoBogus;

using FluentAssertions;

using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

using NSubstitute;

namespace Holefeeder.UnitTests.Application.SeedWork;

public class UserContextTests
{
    private readonly Guid _idNameIdentifier = AutoFaker.Generate<Guid>();
    private readonly Guid _idSub = AutoFaker.Generate<Guid>();

    [Fact]
    public void GivenGetUserId_WhenUserHasNameIdentifierClaim_ThenReturnNameIdentifierClaim()
    {
        // arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _idNameIdentifier.ToString())
            }))
        };

        // act
        var userContext = new UserContext(httpContextAccessor);

        // assert
        userContext.UserId.Should().Be(_idNameIdentifier);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasSubClaim_ThenReturnSubClaim()
    {
        // arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimConstants.Sub, _idSub.ToString())}))
        };

        // act
        var userContext = new UserContext(httpContextAccessor);

        // assert
        userContext.UserId.Should().Be(_idSub);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasNameIdentifierAndSubClaim_ThenReturnNameIdentifierClaim()
    {
        // arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _idNameIdentifier.ToString()),
                new Claim(ClaimConstants.Sub, _idSub.ToString())
            }))
        };

        // act
        var userContext = new UserContext(httpContextAccessor);

        // assert
        userContext.UserId.Should().Be(_idNameIdentifier);
    }

    [Fact]
    public void GivenGetUserId_WhenHttpContextIsNull_ThenReturnEmptyGuid()
    {
        // arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = null;

        // act
        var userContext = new UserContext(httpContextAccessor);

        // assert
        userContext.UserId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasNoClaims_ThenReturnEmptyGuid()
    {
        // arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext {User = new ClaimsPrincipal(new ClaimsIdentity())};

        // act
        var userContext = new UserContext(httpContextAccessor);

        // assert
        userContext.UserId.Should().Be(Guid.Empty);
    }
}

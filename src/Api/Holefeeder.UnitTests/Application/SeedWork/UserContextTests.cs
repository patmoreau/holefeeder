using System.Security.Claims;
using Holefeeder.Application.SeedWork;
using Holefeeder.Tests.Common.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

namespace Holefeeder.UnitTests.Application.SeedWork;

public class UserContextTests
{
    private readonly UserContextDriver _driver = new();

    [Fact]
    public void GivenGetUserId_WhenUserHasNameIdentifierClaim_ThenReturnNameIdentifierClaim()
    {
        // arrange
        var sut = _driver.WhenUserHasNameIdentifierClaim(out var idNameIdentifier).Build();

        // act
        var result = sut.UserId;

        // assert
        result.Should().Be(idNameIdentifier);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasSubClaim_ThenReturnSubClaim()
    {
        // arrange
        var sut = _driver.WhenUserHasSubClaim(out var idSub).Build();

        // act
        var result = sut.UserId;

        // assert
        result.Should().Be(idSub);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasNameIdentifierAndSubClaim_ThenReturnNameIdentifierClaim()
    {
        // arrange
        var sut = _driver.WhenUserHasNameIdentifierAndSubClaim(out var idNameIdentifier).Build();

        // act
        var result = sut.UserId;

        // assert
        result.Should().Be(idNameIdentifier);
    }

    [Fact]
    public void GivenGetUserId_WhenHttpContextIsNull_ThenReturnEmptyGuid()
    {
        // arrange
        var sut = _driver.WhenHttpContextIsNull().Build();

        // act
        var result = sut.UserId;

        // assert
        result.Should().Be(Guid.Empty);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasNoClaims_ThenReturnEmptyGuid()
    {
        // arrange
        var sut = _driver.WhenUserHasNoClaims().Build();

        // act
        var result = sut.UserId;

        // assert
        result.Should().Be(Guid.Empty);
    }

    private class UserContextDriver : IDriverOfT<UserContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        private readonly Guid _idNameIdentifier = AutoFaker.Generate<Guid>();
        private readonly Guid _idSub = AutoFaker.Generate<Guid>();

        public UserContext Build() => new(_httpContextAccessor);

        public UserContextDriver WhenUserHasNameIdentifierClaim(out Guid idNameIdentifier)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _idNameIdentifier.ToString())
                }))
            };
            idNameIdentifier = _idNameIdentifier;

            return this;
        }

        public UserContextDriver WhenUserHasSubClaim(out Guid idSub)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimConstants.Sub, _idSub.ToString())
                }))
            };
            idSub = _idSub;

            return this;
        }

        public UserContextDriver WhenUserHasNameIdentifierAndSubClaim(out Guid idNameIdentifier)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _idNameIdentifier.ToString()),
                    new Claim(ClaimConstants.Sub, _idSub.ToString())
                }))
            };
            idNameIdentifier = _idNameIdentifier;

            return this;
        }

        public UserContextDriver WhenHttpContextIsNull()
        {
            _httpContextAccessor.HttpContext = null;

            return this;
        }

        public UserContextDriver WhenUserHasNoClaims()
        {
            _httpContextAccessor.HttpContext =
                new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };

            return this;
        }
    }
}

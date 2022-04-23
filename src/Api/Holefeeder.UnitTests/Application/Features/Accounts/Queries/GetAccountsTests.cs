using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using Bogus;

using FluentAssertions;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Http;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.Accounts.Queries.GetAccounts;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

public class GetAccountsTests
{
    private readonly Faker<Request> _faker;

    private readonly int _countDummy;
    private readonly IEnumerable<AccountViewModel> _dummy;

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly IAccountQueriesRepository _repositoryMock = Substitute.For<IAccountQueriesRepository>();

    public GetAccountsTests()
    {
        _faker = new AutoFaker<Request>()
            .RuleFor(fake => fake.Offset, fake => fake.Random.Number())
            .RuleFor(fake => fake.Limit, fake => fake.Random.Int(1));

        _countDummy = new Faker().Random.Number(100);
        _dummy = new AutoFaker<AccountViewModel>().Generate(_countDummy);
    }

    [Fact]
    public async Task GivenRequest_WhenBindingFromHttpContext_ThenReturnRequest()
    {
        // arrange
        var httpContext = new DefaultHttpContext
        {
            Request = {QueryString = new QueryString("?offset=10&limit=100&sort=data&filter=code:eq:settings")}
        };

        // act
        var result = await Request.BindAsync(httpContext, null!);

        // assert
        result.Should().BeEquivalentTo(new Request(10, 100, new[] {"data"}, new[] {"code:eq:settings"}));
    }

    [Fact]
    public void GivenValidator_WhenOffsetIsInvalid_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Offset, -1).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Offset);
    }

    [Fact]
    public void GivenValidator_WhenLimitIsInvalid_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Limit, 0).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Limit);
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var request = _faker.Generate();

        _repositoryMock.FindAsync(Arg.Is(_userContextMock.UserId), Arg.Any<QueryParams>(), Arg.Any<CancellationToken>())
            .Returns((_countDummy, _dummy));

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(new QueryResult<AccountViewModel>(_countDummy, _dummy));
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using Bogus;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using FluentAssertions;

using FluentValidation;
using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Http;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.Accounts.Queries.GetAccounts;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

public class GetAccountsQueryTests
{
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
        var request = new Request(-1, QueryParams.DEFAULT_LIMIT, Array.Empty<string>(), Array.Empty<string>());
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Offset)
            .WithErrorMessage("'Offset' must be >= 0.")
            .WithSeverity(Severity.Error)
            .WithErrorCode("GreaterThanOrEqualValidator");
    }

    [Fact]
    public void GivenValidator_WhenLimitIsInvalid_ThenError()
    {
        // arrange
        var request = new Request(QueryParams.DEFAULT_OFFSET, 0, Array.Empty<string>(), Array.Empty<string>());
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Limit)
            .WithErrorMessage("'Limit' must be > 0.")
            .WithSeverity(Severity.Error)
            .WithErrorCode("GreaterThanValidator");
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var count = new Faker().Random.Number(100);
        var userId = AutoFaker.Generate<Guid>();
        var request = new AutoFaker<Request>()
            .RuleFor(fake => fake.Offset, fake => fake.Random.Number())
            .RuleFor(fake => fake.Limit, fake => fake.Random.Int(1))
            .Generate();
        var expected = new AutoFaker<AccountViewModel>().Generate(count);
        var userContextMock = Substitute.For<IUserContext>();
        userContextMock.UserId.Returns(userId);
        var repositoryMock = Substitute.For<IAccountQueriesRepository>();
        repositoryMock.FindAsync(Arg.Is(userId), Arg.Any<QueryParams>(), Arg.Any<CancellationToken>())
            .Returns((count, expected));
        var handler = new Handler(userContextMock, repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(new QueryResult<AccountViewModel>(count, expected));
    }
}

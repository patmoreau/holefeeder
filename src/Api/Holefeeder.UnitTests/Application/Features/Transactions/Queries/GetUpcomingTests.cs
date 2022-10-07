using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using Bogus;

using FluentAssertions;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Transactions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Extensions;
using Holefeeder.Tests.Common.Factories;

using Microsoft.AspNetCore.Http;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.UnitTests.Application.Features.Cashflows.Queries;

public class GetUpcomingTests
{
    private readonly Faker<Request> _faker;
    private readonly IUpcomingQueriesRepository _repositoryMock = Substitute.For<IUpcomingQueriesRepository>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();

    private readonly UpcomingViewModelFactory _viewModelFactory = new();

    public GetUpcomingTests()
    {
        _faker = new AutoFaker<Request>()
            .RuleFor(fake => fake.From, fake => fake.Date.Past().Date)
            .RuleFor(fake => fake.To, fake => fake.Date.Future().Date);
    }

    [Fact]
    public async Task GivenRequest_WhenBindingFromHttpContext_ThenReturnRequest()
    {
        // arrange
        var request = _faker.Generate();
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                QueryString =
                    new QueryString($"?from={request.From.ToPersistent()}&to={request.To.ToPersistent()}")
            }
        };

        // act
        var result = await Request.BindAsync(httpContext, null!);

        // assert
        result.Should().BeEquivalentTo(new Request(request.From, request.To));
    }

    [Fact]
    public void GivenValidator_WhenFromIsNull_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.From, _ => default).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.From);
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var request = _faker.Generate();
        var count = new Faker().Random.Number(100);
        var models = _viewModelFactory.Generate(count);

        _repositoryMock.GetUpcomingAsync(Arg.Is(_userContextMock.UserId), Arg.Any<DateTime>(), Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(models);

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().BeEquivalentTo(new QueryResult<UpcomingViewModel>(count, models));
    }
}

using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Holefeeder.Application.Features.Transactions;
using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Tests.Common.Factories;

using NSubstitute;

using static Holefeeder.Application.Features.Transactions.Queries.GetTransaction;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

public class GetTransactionTests
{
    private readonly AutoFaker<Request> _faker = new();
    private readonly ITransactionQueriesRepository _repositoryMock = Substitute.For<ITransactionQueriesRepository>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();

    private readonly TransactionInfoViewModelFactory _viewModelFactory = new();

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenHandler_WhenIdNotFound_ThenThrowException()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<TransactionNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var request = _faker.Generate();
        var transaction = _viewModelFactory.Generate();

        _repositoryMock.FindByIdAsync(Arg.Is(_userContextMock.UserId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(transaction);

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(transaction);
    }
}

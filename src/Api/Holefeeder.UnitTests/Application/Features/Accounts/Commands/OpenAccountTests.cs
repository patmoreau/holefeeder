using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using Bogus;

using FluentAssertions;
using FluentAssertions.Execution;

using FluentValidation.TestHelper;

using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

public class OpenAccountTests
{
    private readonly Faker<Request> _faker;

    private readonly string _name = AutoFaker.Generate<string>();
    private readonly Account _dummy = new AutoFaker<Account>().Generate();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly IAccountRepository _repositoryMock = Substitute.For<IAccountRepository>();

    public OpenAccountTests()
    {
        _faker = new AutoFaker<Request>()
            .RuleForType(typeof(AccountType), faker => faker.PickRandom(AccountType.List.ToArray()));
    }

    [Fact]
    public async Task GivenValidator_WhenTypeIsNull_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Type, _ => null!).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GivenValidator_WhenNameIsInvalid_ThenError(string name)
    {
        // arrange
        var request = _faker.RuleFor(x => x.Name, _ => name).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenValidator_WhenNameAlreadyExists_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Name, _ => _name).Generate();
        _repositoryMock
            .FindByNameAsync(Arg.Is(_name), Arg.Is(_userContextMock.UserId), Arg.Any<CancellationToken>())
            .Returns(_dummy);

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenValidator_WhenOpenDateIsInvalid_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.OpenDate, _ => DateTime.MinValue).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.OpenDate);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = _faker.Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenHandler_WhenRequestValid_ThenReturnId()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction()
    {
        // arrange
        var request = _faker.Generate();
        _repositoryMock.SaveAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>()).Throws(
            new AccountDomainException(nameof(GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction)));

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        using var scope = new AssertionScope();
        await action.Should().ThrowAsync<AccountDomainException>();
        _repositoryMock.UnitOfWork.Received(1).Dispose();
    }
}

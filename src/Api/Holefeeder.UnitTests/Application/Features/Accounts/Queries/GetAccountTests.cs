using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.SeedWork;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.Accounts.Queries.GetAccount.GetAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

public class GetAccountTests
{
    private readonly AccountViewModel _dummy = new AutoFaker<AccountViewModel>().Generate();
    private readonly AutoFaker<Request> _faker = new();
    private readonly IAccountQueriesRepository _repositoryMock = Substitute.For<IAccountQueriesRepository>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();

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
}

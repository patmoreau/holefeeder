using System;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts;

public class FavoriteCommandValidatorTests
{
    private readonly FavoriteAccount.Validator _validator;

    public FavoriteCommandValidatorTests()
    {
        var logger = Substitute.For<ILogger<FavoriteAccount.Validator>>();
        _validator = new FavoriteAccount.Validator(logger);
    }

    [Fact]
    public void GivenFavoriteAccountValidator_WhenIdIsEmpty_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new FavoriteAccount.Request(Guid.Empty, true));
        result.ShouldHaveValidationErrorFor(m => m.Id);
    }
}

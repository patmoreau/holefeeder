using System;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts;

public class CloseCommandValidatorTests
{
    private readonly CloseAccount.Validator _validator;

    public CloseCommandValidatorTests()
    {
        var logger = Substitute.For<ILogger<CloseAccount.Validator>>();
        _validator = new CloseAccount.Validator(logger);
    }

    [Fact]
    public void GivenCloseAccountValidator_WhenIdIsEmpty_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new CloseAccount.Request(Guid.Empty));
        result.ShouldHaveValidationErrorFor(m => m.Id);
    }

    [Fact]
    public void GivenCloseAccountValidator_WhenIdIsNull_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new CloseAccount.Request(default));
        result.ShouldHaveValidationErrorFor(m => m.Id);
    }
}

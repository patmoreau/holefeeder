using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts;

public class ModifyAccountCommandValidatorTests
{
    private const string LONG_STRING
        = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%*(){}|[]\\;':\",./<>?";

    private readonly ModifyAccount.Validator _validator;

    public ModifyAccountCommandValidatorTests()
    {
        var logger = Substitute.For<ILogger<ModifyAccount.Validator>>();
        _validator = new ModifyAccount.Validator(logger);
    }

    public static IEnumerable<object[]> InvalidNames
    {
        get
        {
            yield return new object[] {""};
            yield return new object[] {"           "};
            yield return new object[] {string.Concat(LONG_STRING, LONG_STRING, LONG_STRING)};
            yield return new object[] {null!};
        }
    }

    [Fact]
    public void GivenModifyAccountValidator_WhenIdIsInvalid_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new ModifyAccount.Request(Guid.Empty, "name", 1, "description"));
        result.ShouldHaveValidationErrorFor(m => m.Id);
    }

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public void GivenModifyAccountValidator_WhenNameIsInvalid_ThenShouldHaveError(string name)
    {
        var result = _validator.TestValidate(new ModifyAccount.Request(Guid.NewGuid(), name, 1, "description"));
        result.ShouldHaveValidationErrorFor(m => m.Name);
    }
}

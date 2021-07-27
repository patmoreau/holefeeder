using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class ModifyAccountCommandValidatorTests
    {
        private readonly ModifyAccountValidator _validator;

        public ModifyAccountCommandValidatorTests()
        {
            var logger = Substitute.For<ILogger<ModifyAccountValidator>>();
            _validator = new ModifyAccountValidator(logger);
        }

        [Fact]
        public void GivenModifyAccountValidator_WhenIdIsInvalid_ThenShouldHaveError()
        {
            var result = _validator.TestValidate(new ModifyAccountCommand { Id = Guid.Empty });
            result.ShouldHaveValidationErrorFor(m => m.Id);
        }

        [Theory, MemberData(nameof(InvalidNames))]
        public void GivenModifyAccountValidator_WhenNameIsInvalid_ThenShouldHaveError(string name)
        {
            var result = _validator.TestValidate(new ModifyAccountCommand { Name = name });
            result.ShouldHaveValidationErrorFor(m => m.Name);
        }

        private const string LONG_STRING
            = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%*(){}|[]\\;':\",./<>?";

        public static IEnumerable<object[]> InvalidNames
        {
            get
            {
                yield return new object[] { "" };
                yield return new object[] { "           " };
                yield return new object[] { string.Concat(LONG_STRING, LONG_STRING, LONG_STRING) };
                yield return new object[] { null };
            }
        }
    }
}

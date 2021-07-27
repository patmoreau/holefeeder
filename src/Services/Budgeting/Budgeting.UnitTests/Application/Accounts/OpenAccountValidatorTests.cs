using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Validators;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class OpenCommandValidatorTests
    {
        private readonly OpenAccountValidator _validator;

        public OpenCommandValidatorTests()
        {
            var logger = Substitute.For<ILogger<OpenAccountValidator>>();
            _validator = new OpenAccountValidator(logger);
        }

        [Theory, MemberData(nameof(InvalidNames))]
        public void GivenOpenAccountValidator_WhenNameIsInvalid_ThenShouldHaveError(string name)
        {
            var result = _validator.TestValidate(new OpenAccountCommand { Name = name });
            result.ShouldHaveValidationErrorFor(m => m.Name);
        }

        [Theory, MemberData(nameof(InvalidDateTimes))]
        public void GivenOpenAccountValidator_WhenOpenDateIsInvalid_ThenShouldHaveError(DateTime openDate)
        {
            var result = _validator.TestValidate(new OpenAccountCommand { OpenDate = openDate });
            result.ShouldHaveValidationErrorFor(m => m.OpenDate);
        }

        [Theory, MemberData(nameof(InvalidTypes))]
        public void GivenOpenAccountValidator_WhenTypeIsInvalid_ThenShouldHaveError(AccountType type)
        {
            var result = _validator.TestValidate(new OpenAccountCommand { Type = type });
            result.ShouldHaveValidationErrorFor(m => m.Type);
        }

        private const string longString
            = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%*(){}|[]\\;':\",./<>?";

        public static IEnumerable<object[]> InvalidNames
        {
            get
            {
                yield return new object[] { "" };
                yield return new object[] { "           " };
                yield return new object[] { string.Concat(longString, longString, longString) };
                yield return new object[] { null };
            }
        }

        public static IEnumerable<object[]> InvalidDateTimes
        {
            get
            {
                yield return new object[] { DateTime.MinValue };
                yield return new object[] { default(DateTime) };
                yield return new object[] { null };
            }
        }

        public static IEnumerable<object[]> InvalidTypes
        {
            get
            {
                yield return new object[] { default(AccountType) };
                yield return new object[] { null };
            }
        }
    }
}

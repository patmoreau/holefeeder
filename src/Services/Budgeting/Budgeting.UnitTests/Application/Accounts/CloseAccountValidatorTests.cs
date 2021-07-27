using System;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class CloseCommandValidatorTests
    {
        private readonly CloseAccountValidator _validator;

        public CloseCommandValidatorTests()
        {
            var logger = Substitute.For<ILogger<CloseAccountValidator>>();
            _validator = new CloseAccountValidator(logger);
        }

        [Fact]
        public void GivenCloseAccountValidator_WhenIdIsEmpty_ThenShouldHaveError()
        {
            var result = _validator.TestValidate(new CloseAccountCommand { Id = Guid.Empty });
            result.ShouldHaveValidationErrorFor(m => m.Id);
        }

        [Fact]
        public void GivenCloseAccountValidator_WhenIdIsNull_ThenShouldHaveError()
        {
            var result = _validator.TestValidate(new CloseAccountCommand());
            result.ShouldHaveValidationErrorFor(m => m.Id);
        }
    }
}

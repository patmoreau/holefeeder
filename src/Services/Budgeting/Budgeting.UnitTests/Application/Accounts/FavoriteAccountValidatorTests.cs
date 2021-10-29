using System;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class FavoriteCommandValidatorTests
    {
        private readonly FavoriteAccountValidator _validator;

        public FavoriteCommandValidatorTests()
        {
            var logger = Substitute.For<ILogger<FavoriteAccountValidator>>();
            _validator = new FavoriteAccountValidator(logger);
        }

        [Fact]
        public void GivenFavoriteAccountValidator_WhenIdIsEmpty_ThenShouldHaveError()
        {
            var result = _validator.TestValidate(new FavoriteAccountCommand { Id = Guid.Empty, IsFavorite = true });
            result.ShouldHaveValidationErrorFor(m => m.Id);
        }
    }
}

using DrifterApps.Holefeeder.ObjectStore.Application.Validators;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace ObjectStore.UnitTests.Application
{
    public class CreateStoreItemCommandValidatorTests
    {
        private readonly CreateStoreItemCommandValidator _validator;

        public CreateStoreItemCommandValidatorTests()
        {
            var logger = Substitute.For<ILogger<CreateStoreItemCommandValidator>>();
            _validator = new CreateStoreItemCommandValidator(logger);
        }

        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenCodeIsEmpty_ThenShouldHaveError()
        {
            var result = _validator.TestValidate(new CreateStoreItemCommand { Code = null! });
            result.ShouldHaveValidationErrorFor(m => m.Code);
        }

        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenDataIsEmpty_ThenShouldHaveError()
        {
            var result = _validator.TestValidate(new CreateStoreItemCommand { Data = null! });
            result.ShouldHaveValidationErrorFor(m => m.Data);
        }
    }
}

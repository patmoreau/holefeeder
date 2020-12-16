using DrifterApps.Holefeeder.ObjectStore.Application.Validators;

using FluentValidation.TestHelper;

using Xunit;

namespace ObjectStore.UnitTests.Application
{
    public class CreateStoreItemCommandValidatorTests
    {
        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenCodeIsEmpty_ThenShouldHaveError()
        {
            var validator = new CreateStoreItemCommandValidator();
            validator.ShouldHaveValidationErrorFor(m => m.Code, null as string);
        }

        [Fact]
        public void GivenCreateStoreItemCommandValidator_WhenDataIsEmpty_ThenShouldHaveError()
        {
            var validator = new CreateStoreItemCommandValidator();
            validator.ShouldHaveValidationErrorFor(m => m.Data, null as string);
        }
    }
}

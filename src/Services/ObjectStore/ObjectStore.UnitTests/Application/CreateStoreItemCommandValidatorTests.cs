using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;

using FluentValidation.TestHelper;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace ObjectStore.UnitTests.Application;

public class CreateStoreItemCommandValidatorTests
{
    private readonly CreateStoreItem.Validator _validator;

    public CreateStoreItemCommandValidatorTests()
    {
        var logger = Substitute.For<ILogger<CreateStoreItem.Validator>>();
        _validator = new CreateStoreItem.Validator(logger);
    }

    [Fact]
    public void GivenCreateStoreItemCommandValidator_WhenCodeIsEmpty_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new CreateStoreItem.Request(null!, string.Empty));
        result.ShouldHaveValidationErrorFor(m => m.Code);
    }

    [Fact]
    public void GivenCreateStoreItemCommandValidator_WhenDataIsEmpty_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new CreateStoreItem.Request(string.Empty, null!));
        result.ShouldHaveValidationErrorFor(m => m.Data);
    }
}

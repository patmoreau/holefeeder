using System;

using AutoBogus;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

using Xunit;

using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem.ModifyStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

public class ModifyStoreItemTests
{
    private readonly AutoFaker<Request> _faker = new();

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public void GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Data, string.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Data);
    }

    [Fact]
    public void GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = _faker.Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

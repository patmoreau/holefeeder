using System;

using AutoBogus;

using Bogus;

using FluentValidation.TestHelper;

using Xunit;

using static Holefeeder.Application.Features.StoreItems.Queries.GetStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Queries;

public class GetStoreItemTests
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

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
}

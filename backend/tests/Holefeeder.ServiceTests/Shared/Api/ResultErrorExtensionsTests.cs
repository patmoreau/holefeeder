using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holefeeder.ServiceTests.Shared.Api;

public class ResultErrorExtensionsTests
{
    private static readonly ResultError AnError = new("an-error", "Something went wrong.");

    [Theory]
    [InlineData(StatusCodes.Status400BadRequest)]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status500InternalServerError)]
    public void ToProblemDetails_ReturnsProblemWithGivenStatusCode(int statusCode)
    {
        var result = AnError.ToProblemDetails(statusCode);

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(statusCode);
        problem.ProblemDetails.Status.Should().Be(statusCode);
    }

    [Fact]
    public void ToProblemDetails_UsesErrorDescriptionAsDetail()
    {
        var result = AnError.ToProblemDetails(StatusCodes.Status400BadRequest);

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.ProblemDetails.Detail.Should().Be("Something went wrong.");
    }

    [Fact]
    public void ToValidationProblemDetails_ReturnsBadRequestValidationProblem()
    {
        var error = new ResultErrorAggregate("validation", "One or more validation errors occurred.",
            new Dictionary<string, string[]> { ["limit"] = ["Limit is invalid."] });

        var result = error.ToValidationProblemDetails();

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>();
    }

    [Fact]
    public void ToValidationProblemDetails_UsesAggregateDescriptionAsDetail()
    {
        var error = new ResultErrorAggregate("validation", "One or more validation errors occurred.",
            new Dictionary<string, string[]> { ["limit"] = ["Limit is invalid."] });

        var result = error.ToValidationProblemDetails();

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.ProblemDetails.Detail.Should().Be("One or more validation errors occurred.");
    }

    [Fact]
    public void ToValidationProblemDetails_CopiesEveryFieldError()
    {
        var error = new ResultErrorAggregate("validation", "One or more validation errors occurred.",
            new Dictionary<string, string[]>
            {
                ["limit"] = ["Limit is invalid."],
                ["offset"] = ["Offset is invalid.", "Offset is not a number."]
            });

        var result = error.ToValidationProblemDetails();

        var details = result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>().Subject;
        details.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
        {
            ["limit"] = ["Limit is invalid."],
            ["offset"] = ["Offset is invalid.", "Offset is not a number."]
        });
    }

    [Fact]
    public void ToValidationProblemDetails_ReturnsNoFieldErrors_WhenAggregateIsEmpty()
    {
        var error = new ResultErrorAggregate("validation", "One or more validation errors occurred.",
            new Dictionary<string, string[]>());

        var result = error.ToValidationProblemDetails();

        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>()
            .Which.Errors.Should().BeEmpty();
    }
}

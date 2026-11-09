using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holefeeder.ServiceTests.Shared.Api;

public class ApiResultTests
{
    private const string ValidationDescription = "One or more validation errors occurred.";

    [Fact]
    public void ToPagedOk_ReturnsOkWithTheItems()
    {
        string[] items = ["first", "second"];

        var result = ApiResult.ToPagedOk(5, items, new DefaultHttpContext());

        result.Should().BeOfType<Ok<IEnumerable<string>>>()
            .Which.Value.Should().Equal("first", "second");
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(5, "5")]
    [InlineData(12345, "12345")]
    public void ToPagedOk_SetsTotalCountHeader(int total, string expectedHeader)
    {
        var context = new DefaultHttpContext();

        ApiResult.ToPagedOk(total, Array.Empty<string>(), context);

        context.Response.Headers["X-Total-Count"].ToString().Should().Be(expectedHeader);
    }

    [Fact]
    public void ToProblem_ReturnsBadRequestValidationProblem_WhenErrorIsAValidationAggregate()
    {
        var error = new ResultErrorAggregate("validation", ValidationDescription,
            new Dictionary<string, string[]> { ["limit-invalid"] = ["Limit is invalid."] });

        var result = ApiResult.ToProblem(error);

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>();
        problem.ProblemDetails.Detail.Should().Be(ValidationDescription);
    }

    [Fact]
    public void ToProblem_CopiesEveryFieldError_WhenErrorIsAValidationAggregate()
    {
        var error = new ResultErrorAggregate("validation", ValidationDescription,
            new Dictionary<string, string[]>
            {
                ["limit-invalid"] = ["Limit is invalid."],
                ["offset-invalid"] = ["Offset is invalid.", "Offset is not a number."]
            });

        var result = ApiResult.ToProblem(error);

        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>()
            .Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                ["limit-invalid"] = ["Limit is invalid."],
                ["offset-invalid"] = ["Offset is invalid.", "Offset is not a number."]
            });
    }

    [Fact]
    public void ToProblem_ReturnsNoFieldErrors_WhenValidationAggregateIsEmpty()
    {
        var error = new ResultErrorAggregate("validation", ValidationDescription, new Dictionary<string, string[]>());

        var result = ApiResult.ToProblem(error);

        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>()
            .Which.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ToProblem_ReturnsServerError_WhenErrorIsAnUnexpectedError()
    {
        var error = new UnexpectedError("things-read-failed", "Things could not be read from the database.");

        var result = ApiResult.ToProblem(error);

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problem.ProblemDetails.Detail.Should().Be("Things could not be read from the database.");
    }

    [Fact]
    public void ToProblem_ReturnsNotFound_WhenErrorIsANotFoundError()
    {
        var error = new NotFoundError("thing-not-found", "Thing was not found.");

        var result = ApiResult.ToProblem(error);

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        problem.ProblemDetails.Detail.Should().Be("Thing was not found.");
    }

    [Fact]
    public void ToProblem_ReturnsBadRequest_WhenErrorIsAnyOtherError()
    {
        var error = new ResultError("thing-invalid", "Thing is invalid.");

        var result = ApiResult.ToProblem(error);

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Detail.Should().Be("Thing is invalid.");
    }
}

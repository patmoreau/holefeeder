using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Api;
using Holefeeder.Service.StoreItems.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holefeeder.ServiceTests.StoreItems.Api;

public class ResultErrorExtensionsTests
{
    [Fact]
    public void ToProblem_ReturnsValidationProblem_WhenErrorIsAValidationAggregate()
    {
        var error = new ResultErrorAggregate("validation", "One or more validation errors occurred.",
            new Dictionary<string, string[]> { [OffsetErrors.CodeInvalid] = [OffsetErrors.Invalid.Description] });

        var result = error.ToProblem();

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>()
            .Which.Errors.Should().ContainKey(OffsetErrors.CodeInvalid);
    }

    [Fact]
    public void ToProblem_ReturnsServerError_WhenReadFailed()
    {
        var result = StoreItemsErrors.ReadFailed.ToProblem();

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problem.ProblemDetails.Detail.Should().Be(StoreItemsErrors.ReadFailed.Description);
    }

    [Fact]
    public void ToProblem_ReturnsServerError_WhenErrorIsUnknown()
    {
        var result = new ResultError("unknown", "Something unexpected happened.").ToProblem();

        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problem.ProblemDetails.Detail.Should().Be("Something unexpected happened.");
    }
}

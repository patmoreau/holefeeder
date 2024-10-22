using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing.Attributes;

using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.UnitTests.Domain.Extensions;

public static class ResultHelperTests
{
    [AssertionMethod]
    internal static void ShouldHaveError<T>(this Result<T> result, ResultError error) =>
        result.Should()
            .BeFailure()
            .And.Subject
            .Error.Should().BeOfType<ResultErrorAggregate>()
            .Which
            .Errors.Should().ContainKey(error.Code)
            .WhoseValue.Should().ContainSingle(error.Description);

}

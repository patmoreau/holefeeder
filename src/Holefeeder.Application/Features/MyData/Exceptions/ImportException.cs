using DrifterApps.Seeds.Domain;

namespace Holefeeder.Application.Features.MyData.Exceptions;

#pragma warning disable CA1032
public class ImportException(ResultError error) : Exception($"{error.Code}: {error.Description}")
#pragma warning restore CA1032
{
    public ResultError Error { get; } = error;
}

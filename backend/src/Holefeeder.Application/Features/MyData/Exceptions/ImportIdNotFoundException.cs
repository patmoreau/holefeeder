using Holefeeder.Application.Exceptions;

namespace Holefeeder.Application.Features.MyData.Exceptions;

#pragma warning disable CA1032
public class ImportIdNotFoundException(Guid id) : NotFoundException(id, nameof(MyData))
#pragma warning restore CA1032
{
}

using Holefeeder.Application.Exceptions;

namespace Holefeeder.Application.Features.MyData.Exceptions;

#pragma warning disable CA1032
public class ImportIdNotFoundException : NotFoundException
#pragma warning restore CA1032
{
    public ImportIdNotFoundException(Guid id) : base(id, nameof(MyData))
    {
    }
}

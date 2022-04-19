using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.MyData.Exceptions;

public class ImportIdNotFoundException : DomainException
{
    public ImportIdNotFoundException(Guid id) : base(StatusCodes.Status404NotFound,
        $"{nameof(ImportDataStatusDto)} '{id}' not found")
    {
    }

    public override string Context => nameof(ImportIdNotFoundException);
}

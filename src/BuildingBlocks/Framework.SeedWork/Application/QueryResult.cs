using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application
{
    public record QueryResult<T>(int TotalCount, IEnumerable<T> Items);
}

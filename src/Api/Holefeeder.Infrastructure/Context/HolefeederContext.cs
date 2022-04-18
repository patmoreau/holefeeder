using Holefeeder.Infrastructure.SeedWork;

namespace Holefeeder.Infrastructure.Context;

public class HolefeederContext : MySqlDbContext, IHolefeederContext
{
    public HolefeederContext(HolefeederDatabaseSettings settings) : base(settings)
    {
    }
}

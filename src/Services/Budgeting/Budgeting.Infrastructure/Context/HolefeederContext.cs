using Framework.Dapper.SeedWork;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public class HolefeederContext : MySqlDbContext, IHolefeederContext
    {
        public HolefeederContext(HolefeederDatabaseSettings settings) : base(settings)
        {
        }
    }
}

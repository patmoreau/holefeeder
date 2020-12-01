namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
{
    public class HolefeederDatabaseSettings : IHolefeederDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}

namespace DrifterApps.Holefeeder.Infrastructure.Database
{
    public class HolefeederDatabaseSettings : IHolefeederDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}

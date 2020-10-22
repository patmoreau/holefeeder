namespace DrifterApps.Holefeeder.Infrastructure.Database
{
    public interface IHolefeederDatabaseSettings
    {
        public string ConnectionString { get; }
        public string Database { get; }
    }
}

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure
{
    public interface IHolefeederDatabaseSettings
    {
        public string ConnectionString { get; }
        public string Database { get; }
    }
}

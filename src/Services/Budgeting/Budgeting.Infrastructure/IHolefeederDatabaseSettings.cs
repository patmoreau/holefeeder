namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
{
    public interface IHolefeederDatabaseSettings
    {
        public string ConnectionString { get; }
        public string Database { get; }
    }
}

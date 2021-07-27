namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public record MongoDatabaseSettings
    {
        public string ConnectionString { get; init; }
        public string Database { get; init; }
        public bool Migrate { get; init; }
    }
}

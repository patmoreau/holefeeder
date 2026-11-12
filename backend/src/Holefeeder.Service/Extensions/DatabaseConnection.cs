namespace Holefeeder.Service.Extensions;

internal static class DatabaseConnection
{
    private const string ConnectionStringName = "BudgetingConnectionString";

    internal static string StringFrom(IServiceProvider provider) =>
        provider.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName)!;
}

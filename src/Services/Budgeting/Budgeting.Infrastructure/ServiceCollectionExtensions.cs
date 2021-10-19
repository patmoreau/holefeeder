using System;
using System.Reflection;

using Dapper;

using DbUp;
using DbUp.MySql;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Application.Cashflows;
using DrifterApps.Holefeeder.Budgeting.Application.Categories;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Scripts;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MySqlConnector;

using MySqlConnectionManager = Framework.Dapper.SeedWork.MySqlConnectionManager;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        private static readonly object Locker = new();

        public static IServiceCollection AddHolefeederDatabase(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<HolefeederDatabaseSettings>(
                configuration.GetSection(nameof(HolefeederDatabaseSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);

            services.AddScoped<IHolefeederContext, HolefeederContext>();
            services.AddScoped<Script000InitDatabase>();
            services.AddScoped<Script005MongoDbMigration>();

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountQueriesRepository, AccountQueriesRepository>();
            services.AddTransient<ICashflowQueriesRepository, CashflowQueriesRepository>();
            services.AddTransient<ICategoryQueriesRepository, CategoriesQueriesRepository>();
            services.AddTransient<ICategoriesRepository, CategoriesQueriesRepository>();
            services.AddTransient<ITransactionQueriesRepository, TransactionQueriesRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

            services.AddAutoMapper(typeof(MappingProfile));

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            SqlMapper.AddTypeHandler(new AccountTypeHandler());
            SqlMapper.AddTypeHandler(new CategoryTypeHandler());
            SqlMapper.AddTypeHandler(new DateIntervalTypeHandler());
            SqlMapper.AddTypeHandler(new TagsHandler());

            return services;
        }

        public static IApplicationBuilder MigrateDb(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var databaseSettings = scope.ServiceProvider.GetRequiredService<HolefeederDatabaseSettings>();

            // var initDatabaseScript = scope.ServiceProvider.GetRequiredService<Script000InitDatabase>();
            // var mongoDbMigration = scope.ServiceProvider.GetRequiredService<Script005MongoDbMigration>();

            lock (Locker)
            {
                var connectionStringBuilder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);

                var connectionManager = new MySqlConnectionManager(databaseSettings.ConnectionString);

                // EnsureDatabase.For.MySqlDatabase(databaseSettings.BasicConnectionString);

                var upgradeEngine = DeployChanges.To
                    .MySqlDatabase(connectionManager)
                    // .WithScript(Script000InitDatabase.ScriptName, initDatabaseScript)
                    // .WithScript(Script005MongoDbMigration.ScriptName, mongoDbMigration)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .JournalTo(new MySqlTableJournal(
                        () => connectionManager,
                        () => MySqlConnectionManager.Log, connectionStringBuilder.Database, "schema_versions"))
                    .LogToConsole()
                    .Build();

                var result = upgradeEngine.PerformUpgrade();
                if (!result.Successful)
                {
                    throw result.Error;
                }
            }

            return app;
        }
    }
}

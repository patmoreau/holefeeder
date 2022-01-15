using System;
using System.Reflection;

using Dapper;

using DbUp;
using DbUp.MySql;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Application.Cashflows;
using DrifterApps.Holefeeder.Budgeting.Application.Categories;
using DrifterApps.Holefeeder.Budgeting.Application.MyData;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MySqlConnectionManager = Framework.Dapper.SeedWork.MySqlConnectionManager;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        private static readonly object Locker = new();

        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<HolefeederDatabaseSettings>(
                configuration.GetSection(nameof(HolefeederDatabaseSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);

            services.AddSingleton<AccountMapper>();
            services.AddSingleton<CashflowMapper>();
            services.AddSingleton<CategoryMapper>();
            services.AddSingleton<TagsMapper>();
            services.AddSingleton<TransactionMapper>();

            services.AddScoped<IHolefeederContext, HolefeederContext>();

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountQueriesRepository, AccountQueriesRepository>();
            services.AddTransient<ICashflowQueriesRepository, CashflowQueriesRepository>();
            services.AddTransient<ICashflowRepository, CashflowRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ICategoryQueriesRepository, CategoriesQueriesRepository>();
            services.AddTransient<ICategoriesRepository, CategoriesQueriesRepository>();
            services.AddTransient<IMyDataQueriesRepository, MyDataQueriesRepository>();
            services.AddTransient<ITransactionQueriesRepository, TransactionQueriesRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

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

            lock (Locker)
            {
                var connectionManager = new MySqlConnectionManager(databaseSettings.ConnectionString);

                EnsureDatabase.For.MySqlDatabase(databaseSettings.GetBuilder(true).ConnectionString);

                var upgradeEngine = DeployChanges.To
                    .MySqlDatabase(connectionManager)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .JournalTo(new MySqlTableJournal(
                        () => connectionManager,
                        () => MySqlConnectionManager.Log, databaseSettings.GetBuilder().Database, "schema_versions"))
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

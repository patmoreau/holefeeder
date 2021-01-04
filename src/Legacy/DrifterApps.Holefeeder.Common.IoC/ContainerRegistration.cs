using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Common.IoC
{
    public static class ContainerRegistration
    {
        public static void Initialize(this IServiceCollection container, IConfiguration configuration)
        {
            container.ThrowIfNull(nameof(container));
            configuration.ThrowIfNull(nameof(configuration));

            // add singleton
            container.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            // add AutoMapper
            var mapperConfig = MapperRegistration.Initialize();
            container.AddScoped(provider => mapperConfig.CreateMapper());

            var mongoConfig = configuration.GetSection("MongoDB");
            
            // add MongoDb components
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            container.AddSingleton<IMongoClient>(provider =>
                new MongoClient(mongoConfig["ConnectionString"]));
            container.AddSingleton(provider =>
                provider.GetService<IMongoClient>().GetDatabase(mongoConfig["DatabaseName"]));
            container.AddScoped(
                provider => provider.GetService<IMongoDatabase>().GetCollection<AccountSchema>("accounts"));
            container.AddScoped(
                provider => provider.GetService<IMongoDatabase>().GetCollection<CashflowSchema>("cashflows"));
            container.AddScoped(
                provider => provider.GetService<IMongoDatabase>().GetCollection<CategorySchema>("categories"));
            container.AddScoped(
                provider => provider.GetService<IMongoDatabase>().GetCollection<ObjectDataSchema>("objectsData"));
            container.AddScoped(
                provider => provider.GetService<IMongoDatabase>().GetCollection<TransactionSchema>("transactions"));

            // add application services
            container.AddScoped<IAccountsService, AccountsService>();
            container.AddScoped<ICashflowsService, CashflowsService>();
            container.AddScoped<ICategoriesService, CategoriesService>();
            container.AddScoped<IObjectsService, ObjectsService>();
            container.AddScoped<IStatisticsService, StatisticsService>();
            container.AddScoped<ITransactionsService, TransactionsService>();

            // add application repositories
            container.AddScoped<IAccountsRepository, AccountsRepository>();
            container.AddScoped<ICashflowsRepository, CashflowsRepository>();
            container.AddScoped<ICategoriesRepository, CategoriesRepository>();
            container.AddScoped<IObjectsRepository, ObjectsRepository>();
            container.AddScoped<ITransactionsRepository, TransactionsRepository>();
        }
    }
}

using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountQueriesRepository, AccountQueriesRepository>();
            services.AddScoped<IUserQueriesRepository, UserQueriesRepository>();
            services.AddScoped<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

            services.Configure<HolefeederDbConfig>()
            var mongoConfig = configuration.GetSection("MongoDB");
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConfig["ConnectionString"]));
            services.AddScoped(provider => provider.GetService<IMongoClient>().GetDatabase(mongoConfig["DatabaseName"]));
            services.AddTransient<IMongoDbContext, MongoDbContext>();

            return services;
        }
    }
}

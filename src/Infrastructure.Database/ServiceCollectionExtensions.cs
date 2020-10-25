using System;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;
using DrifterApps.Holefeeder.Infrastructure.Database.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DrifterApps.Holefeeder.Infrastructure.Database
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHolefeederDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            services.Configure<HolefeederDatabaseSettings>(configuration.GetSection(nameof(HolefeederDatabaseSettings)));

            services.AddSingleton<IHolefeederDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services.AddScoped<IAccountQueriesRepository, AccountQueriesRepository>();
            services.AddScoped<IUserQueriesRepository, UserQueriesRepository>();
            services.AddScoped<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

            return services;
        }
    }
}

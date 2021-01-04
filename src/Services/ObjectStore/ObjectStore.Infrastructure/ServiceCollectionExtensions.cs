using AutoMapper;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddObjectStoreDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            services.Configure<HolefeederDatabaseSettings>(configuration.GetSection(nameof(HolefeederDatabaseSettings)));

            services.AddSingleton<IHolefeederDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStoreQueriesRepository, StoreQueriesRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}

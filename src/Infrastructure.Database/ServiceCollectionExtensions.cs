using AutoMapper;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Transactions.Contracts;
using DrifterApps.Holefeeder.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Domain.SeedWork;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountQueriesRepository, AccountQueriesRepository>();
            services.AddScoped<ICategoryQueries, CategoriesQueriesRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}

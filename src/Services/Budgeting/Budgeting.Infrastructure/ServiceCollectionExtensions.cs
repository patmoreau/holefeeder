using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure
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

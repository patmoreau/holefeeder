using System;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests
{
    public class DatabaseFixture : IDisposable
    {
        public IMongoDbContext DatabaseContext => TestingHost.Services.GetService<IMongoDbContext>();
        
        public IHost TestingHost { get; }

        public DatabaseFixture()
        {
            TestingHost = Host.CreateDefaultBuilder(new[] {"--environment=Development"})
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHolefeederDatabase(hostContext.Configuration);
                }).Build();
        }

        private bool _isDisposed;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                var settings = TestingHost.Services.GetRequiredService<IOptions<HolefeederDatabaseSettings>>();
                if (!string.IsNullOrWhiteSpace(settings.Value.ConnectionString))
                {
                    var client = new MongoClient(settings.Value.ConnectionString);
                    client.DropDatabase(settings.Value.Database);
                }
                TestingHost.Dispose();
            }

            _isDisposed = true;
        }
    }
}

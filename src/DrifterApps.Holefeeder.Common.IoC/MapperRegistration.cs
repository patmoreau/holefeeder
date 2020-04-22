using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo;
using DrifterApps.Holefeeder.Services.Api;

namespace DrifterApps.Holefeeder.Common.IoC
{
    internal static class MapperRegistration
    {
        internal static MapperConfiguration Initialize() =>
            new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AddProfile<ServicesApiProfile>();
                cfg.AddProfile<DataAccessProfile>();
            });
    }
}

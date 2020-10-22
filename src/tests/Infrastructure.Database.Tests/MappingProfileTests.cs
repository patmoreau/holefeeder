using AutoMapper;
using Xunit;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Tests
{
    public class MappingProfileTests
    {
        [Fact]
        public void GivenMappingProfile_WhenCheckingConfiguration_ThenNoErrors()
        {
            var configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(typeof(MappingProfile)));

            configuration.AssertConfigurationIsValid();
        }
    }
}

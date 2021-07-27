using AutoMapper;

using DrifterApps.Holefeeder.ObjectStore.Infrastructure;

using Xunit;

namespace ObjectStore.UnitTests.Infrastructure
{
    public class MappingProfileTests
    {
        [Fact]
        public void GivenMappingProfile_WhenCheckingConfiguration_ThenNoErrors()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));

            configuration.AssertConfigurationIsValid();
        }
    }
}

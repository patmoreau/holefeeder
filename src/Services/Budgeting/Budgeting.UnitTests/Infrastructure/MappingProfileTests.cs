using AutoMapper;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;
using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Infrastructure
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

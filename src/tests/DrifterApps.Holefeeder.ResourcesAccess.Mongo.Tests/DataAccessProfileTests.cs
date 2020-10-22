using System;
using AutoMapper;
using Xunit;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Tests
{
    public class DataAccessProfileTests
    {
        [Fact]
        public void GivenDataAccessProfile_WhenVerifyConfiguration_ThenValid()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DataAccessProfile>();
            });

            configuration.AssertConfigurationIsValid();
        }
    }
}

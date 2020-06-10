using AutoMapper;
using DrifterApps.Holefeeder.Services.BudgetApi;
using Xunit;

namespace DrifterApps.Holefeeder.Services.BudgetAPI.Tests
{
    public class ServicesApiProfileTests
    {
        [Fact]
        public void GivenServiceApiProfile_WhenVerifyConfiguration_ThenValid()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ServicesApiProfile>();
            });

            configuration.AssertConfigurationIsValid();
        }
    }
}

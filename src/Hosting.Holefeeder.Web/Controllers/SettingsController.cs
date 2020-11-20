using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DrifterApps.Holefeeder.Hosting.Holefeeder.Web.Controllers
{
    [Route("assets/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly AppConfig _config;
        
        public SettingsController(IOptions<AppConfig> options)
        {
            _config = options?.Value;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_config);
        }
    }
}

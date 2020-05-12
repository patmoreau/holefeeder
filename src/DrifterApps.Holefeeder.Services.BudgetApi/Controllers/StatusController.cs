using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Services.BudgetDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Services.BudgetApi.Controllers
{
    [Route("api/v1/[controller]")]
    public class StatusController : Controller
    {
        private struct Routes
        {
            public const string GET_STATUS = "get_status";
        }

        public StatusController()
        {
        }

        [HttpGet(Name = Routes.GET_STATUS)]
        [ProducesResponseType(typeof(StatusDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var assembly = Assembly.GetEntryAssembly();
                var status = new StatusDto(assembly?.GetName()?.Version?.ToString() ?? "unknown",
                    assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "unknown",
                    assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown");
                
                return Ok(status);
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}

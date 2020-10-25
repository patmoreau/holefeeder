using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.API.Authorization;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.API.Controllers
{
    [Route("api/v2/[controller]"), Authorize(Policy = Policies.REGISTERED_USER)]
    public class CashflowsController : ControllerRoot
    {
        private struct Routes
        {
            public const string GET_UPCOMING = "get_upcoming";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<CashflowsController> _logger;

        public CashflowsController(IMediator mediator, ILogger<CashflowsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet("upcoming", Name = Routes.GET_UPCOMING)]
        [ProducesResponseType(typeof(UpcomingViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromQuery] DateTime from, [FromQuery] DateTime to,
            CancellationToken cancellationToken = default)
        {
            if (from > to)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new GetUpcomingQuery(UserId, from, to), cancellationToken);

            return Ok(response);
        }
    }
}

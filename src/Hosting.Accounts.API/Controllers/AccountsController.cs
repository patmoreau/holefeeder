using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Hosting.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.API.Controllers
{
    [Authorize(Policy = Policies.REGISTERED_USER)]
    [ApiController]
    [Route("api/v2/[controller]")]
    public class AccountsController : ControllerRoot
    {
        private struct Routes
        {
            public const string GET_ACCOUNTS = "get_accounts";
            public const string GET_ACCOUNT = "get_account";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_ACCOUNTS)]
        [ProducesResponseType(typeof(AccountViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSummaryAsync([FromQuery] int? offset, [FromQuery] int? limit,
            [FromQuery] string[] sort, [FromQuery] string[] filter, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(
                new GetAccountsQuery(UserId, offset ?? 0, limit ?? int.MaxValue, sort, filter),
                cancellationToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}", Name = Routes.GET_ACCOUNT)]
        [ProducesResponseType(typeof(AccountViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(
                new GetAccountQuery(UserId, id),
                cancellationToken);

            return Ok(response);
        }
    }
}

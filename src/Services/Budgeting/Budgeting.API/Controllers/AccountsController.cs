using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    public class AccountsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_ACCOUNTS = "get-accounts";
            public const string GET_ACCOUNT = "get-account";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Routes.GET_ACCOUNTS, Name = Routes.GET_ACCOUNTS)]
        [ProducesResponseType(typeof(AccountViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccounts([FromQuery] int? offset, int? limit, string[] sort,
            string[] filter, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            var response = await _mediator.Send(new GetAccountsQuery(userId, offset, limit, sort, filter),
                cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_ACCOUNT)]
        [ProducesResponseType(typeof(AccountViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccounts(Guid id, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            if (id == default)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new GetAccountQuery(userId, id), cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}

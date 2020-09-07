using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.API.Authorization;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.API.Controllers
{
    [Authorize(Policy = Policies.REGISTERED_USER)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_ACCOUNTS = "get_accounts";
            public const string GET_ACCOUNT = "get_account";
            public const string GET_ACCOUNT_WITH_DETAILS = "get_account_with_details";
            public const string GET_ACCOUNT_TRANSACTIONS = "get_account_transactions";
            public const string POST_ACCOUNT = "post_account";
            public const string PUT_ACCOUNT = "put_account";
            public const string DELETE_ACCOUNT = "delete_account";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet(Name = Routes.GET_ACCOUNTS)]
        [ProducesResponseType(typeof(AccountViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromQuery] int? offset, [FromQuery] int? limit,
            [FromQuery] string[] sort, [FromQuery] string[] filter, CancellationToken cancellationToken = default)
        {
            var userId = Guid.Parse(User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value ?? Guid.Empty.ToString());

            var response = await _mediator.Send(
                new GetAccountsRequest(userId, offset ?? 0, limit ?? int.MaxValue, sort, filter),
                cancellationToken);

            return Ok(response);
        }
    }
}

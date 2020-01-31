using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Services.API.Controllers
{
    [Route("api/v1/[controller]"), Authorize(Policy = Policies.RegisteredUser)]
    public class AccountsController : Controller
    {
        private class Routes
        {
            public const string GET_ACCOUNTS = "get_accounts";
            public const string GET_ACCOUNT = "get_account";
            public const string GET_ACCOUNT_WITH_DETAILS = "get_account_with_details";
            public const string GET_ACCOUNT_TRANSACTIONS = "get_account_transactions";
            public const string POST_ACCOUNT = "post_account";
            public const string PUT_ACCOUNT = "put_account";
            public const string DELETE_ACCOUNT = "delete_account";
        }

        private readonly IAccountsService _service;
        private readonly ITransactionsService _transactionService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountsController(IAccountsService service, ITransactionsService transactionService, IMapper mapper, ILogger<AccountsController> logger)
        {
            _service = service.ThrowIfNull(nameof(service));
            _transactionService = transactionService.ThrowIfNull(nameof(transactionService));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_ACCOUNTS)]
        [ProducesResponseType(typeof(AccountDetailDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            var result = await _service.FindWithDetailsAsync(userId, new QueryParams(offset, limit, sort, filter));
            return Ok(_mapper.Map<AccountDetailDTO[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_ACCOUNT)]
        [ProducesResponseType(typeof(AccountDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get(string id)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountDTO>(result));
        }

        [HttpGet("{id}/details", Name = Routes.GET_ACCOUNT_WITH_DETAILS)]
        [ProducesResponseType(typeof(AccountDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetWithDetails(string id)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            var result = (await _service.FindWithDetailsAsync(userId, new QueryParams(null, null, null, new [] {$"id={id}"}))).SingleOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountDetailDTO>(result));
        }

        [HttpGet("{id}/transactions", Name = Routes.GET_ACCOUNT_TRANSACTIONS)]
        [ProducesResponseType(typeof(TransactionDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransactions(string id, [FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TransactionDTO[]>(_transactionService.FindAsync(userId, new QueryParams(offset, limit, sort, new string[] {$"account='{id}'"}))));
        }

        [HttpPost(Name = Routes.POST_ACCOUNT)]
        [ProducesResponseType(typeof(AccountDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody] AccountDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<AccountEntity>(model));

            return CreatedAtRoute(Routes.GET_ACCOUNT, new { id = result.Id }, _mapper.Map< AccountDTO> (result));
        }

        [HttpPut("{id}", Name = Routes.PUT_ACCOUNT)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] AccountDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateAsync(id, _mapper.Map<AccountEntity>(model));

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_ACCOUNT)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}

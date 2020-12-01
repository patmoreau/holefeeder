using System.Linq;
using System.Net;
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
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Services.BudgetApi.Controllers
{
    [Route("api/v1/[controller]"), Authorize(Policy = "registered_users")]
    public class AccountsController : Controller
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

        private readonly IAccountsService _service;
        private readonly ITransactionsService _transactionService;
        private readonly IMapper _mapper;

        public AccountsController(IAccountsService service, ITransactionsService transactionService, IMapper mapper)
        {
            _service = service.ThrowIfNull(nameof(service));
            _transactionService = transactionService.ThrowIfNull(nameof(transactionService));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        [HttpGet(Name = Routes.GET_ACCOUNTS)]
        [ProducesResponseType(typeof(AccountDetailDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            var result = await _service.FindWithDetailsAsync(userId, new QueryParams(offset, limit, sort, filter), cancellationToken).ConfigureAwait(false);
            return Ok(_mapper.Map<AccountDetailDto[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_ACCOUNT)]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountDto>(result));
        }

        [HttpGet("{id}/details", Name = Routes.GET_ACCOUNT_WITH_DETAILS)]
        [ProducesResponseType(typeof(AccountDetailDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetWithDetailsAsync(string id, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            var result = (await _service.FindWithDetailsAsync(userId, new QueryParams(null, null, null, new [] {$"id={id}"}), cancellationToken).ConfigureAwait(false)).SingleOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountDetailDto>(result));
        }

        [HttpGet("{id}/transactions", Name = Routes.GET_ACCOUNT_TRANSACTIONS)]
        [ProducesResponseType(typeof(TransactionDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransactionsAsync(string id, [FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TransactionDto[]>(_transactionService.FindAsync(userId, new QueryParams(offset, limit, sort, new[] {$"account='{id}'"}), cancellationToken).ConfigureAwait(false)));
        }

        [HttpPost(Name = Routes.POST_ACCOUNT)]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] AccountDto model, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<AccountEntity>(model), cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(Routes.GET_ACCOUNT, new { id = result.Id }, _mapper.Map<AccountDto> (result));
        }

        [HttpPut("{id}", Name = Routes.PUT_ACCOUNT)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] AccountDto model, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateAsync(userId, id, _mapper.Map<AccountEntity>(model), cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_ACCOUNT)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            await _service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}

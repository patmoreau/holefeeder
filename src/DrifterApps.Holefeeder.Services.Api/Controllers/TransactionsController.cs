using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Services.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Services.API.Controllers
{
    [Route("api/v1/[controller]"), Authorize(Policy = Policies.REGISTERED_USER)]
    public class TransactionsController : Controller
    {
        private struct Routes
        {
            public const string GET_TRANSACTIONS = "get_transactions";
            public const string GET_TRANSACTION = "get_transaction";
            public const string POST_TRANSACTION = "post_transaction";
            public const string PUT_TRANSACTION = "put_transaction";
            public const string DELETE_TRANSACTION = "delete_transaction";
        }

        private readonly ITransactionsService _service;
        private readonly IMapper _mapper;

        public TransactionsController(ITransactionsService service, IMapper mapper)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        [HttpGet(Name = Routes.GET_TRANSACTIONS)]
        [ProducesResponseType(typeof(TransactionDetailDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;

            var query = new QueryParams(offset, limit, sort, filter);

            Response?.Headers?.Add("X-Total-Count", $"{await _service.CountAsync(userId, query, cancellationToken).ConfigureAwait(false)}");

            var result = await _service.FindWithDetailsAsync(userId, query, cancellationToken).ConfigureAwait(false);
            return Ok(_mapper.Map<TransactionDetailDto[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TransactionDto>(result));
        }

        [HttpPost(Name = Routes.POST_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] TransactionDto model, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<TransactionEntity>(model), cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(Routes.GET_TRANSACTION, new { id = result.Id }, _mapper.Map<TransactionDto>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_TRANSACTION)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] TransactionDto model, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            await _service.UpdateAsync(id, _mapper.Map<TransactionEntity>(model), cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_TRANSACTION)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}

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
    public class TransactionsController : Controller
    {
        private class Routes
        {
            public const string GET_TRANSACTIONS = "get_transactions";
            public const string GET_TRANSACTION = "get_transaction";
            public const string POST_TRANSACTION = "post_transaction";
            public const string PUT_TRANSACTION = "put_transaction";
            public const string DELETE_TRANSACTION = "delete_transaction";
        }

        private readonly ITransactionsService _service;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TransactionsController(ITransactionsService service, IMapper mapper, ILogger<TransactionsController> logger)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_TRANSACTIONS)]
        [ProducesResponseType(typeof(TransactionDetailDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            var query = new QueryParams(offset, limit, sort, filter);

            Response?.Headers?.Add("X-Total-Count", $"{await _service.CountAsync(userId, query)}");

            var result = await _service.FindWithDetailsAsync(userId, query);
            return Ok(_mapper.Map<TransactionDetailDTO[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromRoute] string id)
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
            return Ok(_mapper.Map<TransactionDTO>(result));
        }

        [HttpPost(Name = Routes.POST_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody] TransactionDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<TransactionEntity>(model));

            return CreatedAtRoute(Routes.GET_TRANSACTION, new { id = result.Id }, _mapper.Map<TransactionDTO>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_TRANSACTION)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] TransactionDTO model)
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

            var result = await _service.FindByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            await _service.UpdateAsync(id, _mapper.Map<TransactionEntity>(model));

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_TRANSACTION)]
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

            var result = _service.FindByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}

using System;
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
    public class CashflowsController : Controller
    {
        private class Routes
        {
            public const string GET_UPCOMING = "get_upcoming";
            public const string GET_CASHFLOWS = "get_cashflows";
            public const string GET_CASHFLOW = "get_cashflow";
            public const string POST_CASHFLOW = "post_cashflow";
            public const string PUT_CASHFLOW = "put_cashflow";
            public const string DELETE_CASHFLOW = "delete_cashflow";
        }

        private readonly ICashflowsService _service;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CashflowsController(ICashflowsService service, IMapper mapper, ILogger<CashflowsController> logger)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet("upcoming", Name = Routes.GET_UPCOMING)]
        [ProducesResponseType(typeof(UpcomingDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromQuery]DateTime from, [FromQuery] DateTime to)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (from > to)
            {
                return BadRequest();
            }

            var result = await _service.GetUpcomingAsync(userId, (from, to));
            return Ok(_mapper.Map<UpcomingDTO[]>(result));
        }

        [HttpGet(Name = Routes.GET_CASHFLOWS)]
        [ProducesResponseType(typeof(CashflowDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            var result = await _service.FindAsync(userId, new QueryParams(offset, limit, sort, filter));
            return Ok(_mapper.Map<CashflowDTO[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_CASHFLOW)]
        [ProducesResponseType(typeof(CashflowDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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
            return Ok(_mapper.Map<CashflowDTO>(result));
        }

        [HttpPost(Name = Routes.POST_CASHFLOW)]
        [ProducesResponseType(typeof(CashflowDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody] CashflowDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<CashflowEntity>(model));

            return CreatedAtRoute(Routes.GET_CASHFLOW, new { id = result.Id }, _mapper.Map<CashflowDTO>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_CASHFLOW)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] CashflowDTO model)
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

            await _service.UpdateAsync(id, _mapper.Map<CashflowEntity>(model));

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_CASHFLOW)]
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

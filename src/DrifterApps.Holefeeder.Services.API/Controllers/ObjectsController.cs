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
    public class ObjectsController : Controller
    {
        private class Routes
        {
            public const string GET_OBJECTS = "get_objects";
            public const string GET_OBJECT = "get_object";
            public const string PUT_OBJECTS = "put_objects";
            public const string POST_OBJECTS = "post_objects";
            public const string DELETE_OBJECTS = "delete_objects";
        }

        private readonly IObjectsService _service;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObjectsController(IObjectsService service, IMapper mapper, ILogger<ObjectsController> logger)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_OBJECTS)]
        [ProducesResponseType(typeof(ObjectDataDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            return Ok(await _service.FindAsync(userId, new QueryParams()));
        }

        [HttpGet("{id}", Name = Routes.GET_OBJECT)]
        [ProducesResponseType(typeof(ObjectDataDTO), (int)HttpStatusCode.OK)]
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
            return Ok(_mapper.Map<ObjectDataDTO>(result));
        }

        [HttpPost(Name = Routes.POST_OBJECTS)]
        [ProducesResponseType(typeof(ObjectDataDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody] ObjectDataDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _service.FindByCodeAsync(userId, model.Code) != null)
            {
                ModelState.AddModelError("duplicateKey", $"object code '{model.Code}' already exists");
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<ObjectDataEntity>(model));

            return CreatedAtRoute(Routes.GET_OBJECT, new { id = result.Id }, _mapper.Map<ObjectDataDTO>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_OBJECTS)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] ObjectDataDTO model)
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

            await _service.UpdateAsync(id, _mapper.Map<ObjectDataEntity>(model));

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_OBJECTS)]
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
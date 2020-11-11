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
    public class ObjectsController : Controller
    {
        private struct Routes
        {
            public const string GET_OBJECTS = "get_objects";
            public const string GET_OBJECT = "get_object";
            public const string PUT_OBJECTS = "put_objects";
            public const string POST_OBJECTS = "post_objects";
            public const string DELETE_OBJECTS = "delete_objects";
        }

        private readonly IObjectsService _service;
        private readonly IMapper _mapper;

        public ObjectsController(IObjectsService service, IMapper mapper)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        [HttpGet(Name = Routes.GET_OBJECTS)]
        [ProducesResponseType(typeof(ObjectDataDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            return Ok(await _service.FindAsync(userId, new QueryParams(), cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{id}", Name = Routes.GET_OBJECT)]
        [ProducesResponseType(typeof(ObjectDataDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken cancellationToken = default)
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
            return Ok(_mapper.Map<ObjectDataDto>(result));
        }

        [HttpPost(Name = Routes.POST_OBJECTS)]
        [ProducesResponseType(typeof(ObjectDataDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] ObjectDataDto model, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            if (model == null && !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _service.FindByCodeAsync(userId, model?.Code, cancellationToken).ConfigureAwait(false) != null)
            {
                ModelState.AddModelError("duplicateKey", $"object code '{model?.Code}' already exists");
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<ObjectDataEntity>(model), cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(Routes.GET_OBJECT, new { id = result.Id }, _mapper.Map<ObjectDataDto>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_OBJECTS)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] ObjectDataDto model, CancellationToken cancellationToken = default)
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

            await _service.UpdateAsync(userId, id, _mapper.Map<ObjectDataEntity>(model), cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_OBJECTS)]
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

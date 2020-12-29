using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.ObjectStore.API.Authorization;
using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Application.Queries;

using FluentValidation;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.ObjectStore.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    public class StoreItemsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_STORE_ITEMS = "get-store-items";
            public const string GET_STORE_ITEM = "get-store-item";
            public const string CREATE_STORE_ITEM = "create-store-item";
            public const string MODIFY_STORE_ITEM = "modify-store-item";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<StoreItemsController> _logger;

        public StoreItemsController(IMediator mediator, ILogger<StoreItemsController> logger)
        {
            _mediator = mediator.ThrowIfNull(nameof(mediator));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_STORE_ITEMS)]
        [ProducesResponseType(typeof(StoreItemViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetStoreItems([FromQuery] int? offset, [FromQuery] int? limit,
            [FromQuery] string[] sort, [FromQuery] string[] filter, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            var response = await _mediator.Send(new GetStoreItemsQuery(userId, offset, limit, sort, filter),
                cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_STORE_ITEM)]
        [ProducesResponseType(typeof(StoreItemViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetStoreItem(Guid id, CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);
            var userId = User.GetUniqueId();

            if (id == default)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new GetStoreItemQuery(userId, id), cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost(Routes.CREATE_STORE_ITEM, Name = Routes.CREATE_STORE_ITEM)]
        [ProducesResponseType(typeof(CommandResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateStoreItem([FromBody] CreateStoreItemCommand command,
            CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);

            if (!ModelState.IsValid)
            {
                return BadRequest(new CommandResult<Guid>(CommandStatus.BadRequest, Guid.Empty, ModelState.Values.Select(x => x.ToString()).ToArray()));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                
                return CreatedAtRoute(Routes.CREATE_STORE_ITEM, result);
            }
            catch (ValidationException e)
            {
                return BadRequest(new CommandResult<Guid>(CommandStatus.BadRequest, Guid.Empty, e));
            }
        }

        [HttpPut(Routes.MODIFY_STORE_ITEM, Name = Routes.MODIFY_STORE_ITEM)]
        [ProducesResponseType(typeof(CommandResult<Unit>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ModifyStoreItem([FromBody] ModifyStoreItemCommand command,
            CancellationToken cancellationToken = default)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(Scopes.ScopeRequiredByApi);

            if (!ModelState.IsValid)
            {
                return BadRequest(new CommandResult<Unit>(CommandStatus.BadRequest, Unit.Value, ModelState.Values.Select(x => x.ToString()).ToArray()));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                
                return Ok(result);
            }
            catch (ValidationException e)
            {
                return BadRequest(new CommandResult<Unit>(CommandStatus.BadRequest, Unit.Value, e));
            }
        }
    }
}

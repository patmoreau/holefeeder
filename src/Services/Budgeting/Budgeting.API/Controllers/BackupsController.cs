using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Exports.Commands;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    [RequiredScope(Scopes.REGISTERED_USER)]
    public class BackupsController : ControllerBase
    {
        private struct Routes
        {
            public const string IMPORT_DATA = "import-data";
        }

        private readonly IMediator _mediator;

        public BackupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Routes.IMPORT_DATA, Name = Routes.IMPORT_DATA)]
        [ProducesResponseType(typeof(CommandResult<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ImportDataCommand([FromBody] ImportDataCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult<int>.Create(CommandStatus.BadRequest, 0,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Ok)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult<int>.Create(CommandStatus.BadRequest, 0,
                    ex.Errors.Select(e => e.ToString())));
            }
        }
    }
}

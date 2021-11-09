using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Imports.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Imports.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Authorize]
    [ApiVersion("2.0")]
    [RequiredScope(Scopes.REGISTERED_USER)]
    public class ImportsController : ControllerRoot
    {
        private struct Routes
        {
            public const string IMPORT_DATA = "import-data";
            public const string IMPORT_DATA_STATUS = "import-data-status";
        }

        private readonly IMediator _mediator;

        public ImportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Routes.IMPORT_DATA, Name = Routes.IMPORT_DATA)]
        [ProducesResponseType(typeof(CommandResult<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ImportDataCommand([FromBody] ImportDataCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToObjectResult(result);
        }

        [HttpGet(Routes.IMPORT_DATA_STATUS + "/{requestId:guid}", Name = Routes.IMPORT_DATA_STATUS)]
        [ProducesResponseType(typeof(CommandResult<ImportDataStatusViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ImportDataStatusQuery([FromRoute] ImportDataStatusQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ToObjectResult(result);
        }
    }
}

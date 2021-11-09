using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class ControllerRoot : ControllerBase
    {
        protected static ObjectResult ToObjectResult(CommandResult result) =>
            result.Status switch
            {
                CommandStatus.Ok => new OkObjectResult(result),
                CommandStatus.InProgress => new OkObjectResult(result),
                CommandStatus.Completed => new OkObjectResult(result),
                CommandStatus.Accepted => new AcceptedAtRouteResult("", result),
                CommandStatus.Conflict => new ConflictObjectResult(result),
                CommandStatus.Created => new CreatedAtRouteResult("", result),
                CommandStatus.NotFound => new NotFoundObjectResult(result),
                CommandStatus.Error => new BadRequestObjectResult(result),
                CommandStatus.BadRequest => new BadRequestObjectResult(result),
                _ => new BadRequestObjectResult(result)
            };
    }
}

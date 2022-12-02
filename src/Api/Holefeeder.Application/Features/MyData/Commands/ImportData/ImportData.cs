using Holefeeder.Application.Features.MyData.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Application.SeedWork.BackgroundRequest;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.MyData.Commands.ImportData;

public class ImportData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/my-data/import-data",
                (Request request, IUserContext userContext, IValidator<Request> validator,
                    CommandsScheduler commandsScheduler) =>
                {
                    var validation = validator.Validate(request);
                    if (!validation.IsValid)
                    {
                        throw new ValidationException(validation.Errors);
                    }

                    var internalRequest = new InternalRequest
                    {
                        RequestId = Guid.NewGuid(),
                        UpdateExisting = request.UpdateExisting,
                        Data = request.Data,
                        UserId = userContext.UserId
                    };
                    commandsScheduler.SendNow(internalRequest, nameof(ImportData));

                    return Results.AcceptedAtRoute(nameof(ImportDataStatus), new {Id = internalRequest.RequestId},
                        new {Id = internalRequest.RequestId});
                })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ImportData))
            .RequireAuthorization();
    }
}

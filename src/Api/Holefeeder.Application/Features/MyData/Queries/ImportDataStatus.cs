using Holefeeder.Application.Features.MyData.Exceptions;
using Holefeeder.Application.Features.MyData.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;

namespace Holefeeder.Application.Features.MyData.Queries;

public class ImportDataStatus : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/my-data/import-status/{id}", async (Guid id, IMediator mediator) =>
            {
                ImportDataStatusDto requestResult = await mediator.Send(new Request(id));
                return Results.Ok(requestResult);
            })
            .Produces<ImportDataStatusDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ImportDataStatus))
            .RequireAuthorization();

    internal record Request(Guid RequestId) : IRequest<ImportDataStatusDto>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.RequestId).NotEmpty();
    }

    internal class Handler : IRequestHandler<Request, ImportDataStatusDto>
    {
        private readonly IMemoryCache _memoryCache;

        public Handler(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public Task<ImportDataStatusDto> Handle(Request request, CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue(request.RequestId, out object? status) && status is ImportDataStatusDto dto)
            {
                return Task.FromResult(dto);
            }

            throw new ImportIdNotFoundException(request.RequestId);
        }
    }
}

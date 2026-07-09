using DrifterApps.Seeds.Application.EndpointFilters;

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
        app.MapGet("api/v2/my-data/import-status/{id:guid}", async (Guid id, IMemoryCache memoryCache, CancellationToken cancellationToken) =>
            {
                var requestResult = await Handle(id, memoryCache, cancellationToken);
                return Results.Ok(requestResult);
            })
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<ImportDataStatusDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ImportDataStatus))
            .RequireAuthorization(Policies.ReadUser);

    private static Task<ImportDataStatusDto> Handle(Guid id, IMemoryCache memoryCache, CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(id, out var status) && status is ImportDataStatusDto dto)
        {
            return Task.FromResult(dto);
        }

        throw new ImportIdNotFoundException(id);
    }

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => x).NotEmpty();
    }
}

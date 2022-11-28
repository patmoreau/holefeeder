using System.Collections.Immutable;

using Carter;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.MyData.Queries;

public sealed class ExportData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/my-data/export-data", async (IMediator mediator) =>
            {
                var requestResult = await mediator.Send(new Request());
                return Results.Ok(requestResult);
            })
            .Produces<ExportDataDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ExportData))
            .RequireAuthorization();
    }

    public record Request : IRequest<ExportDataDto>;

    public class Handler : IRequestHandler<Request, ExportDataDto>
    {
        private readonly IMyDataQueriesRepository _myDataRepository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, IMyDataQueriesRepository myDataRepository)
        {
            _userContext = userContext;
            _myDataRepository = myDataRepository;
        }

        public async Task<ExportDataDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var accounts = await _myDataRepository.ExportAccountsAsync(_userContext.UserId, cancellationToken);
            var categories = await _myDataRepository.ExportCategoriesAsync(_userContext.UserId, cancellationToken);
            var cashflows = await _myDataRepository.ExportCashflowsAsync(_userContext.UserId, cancellationToken);
            var transactions =
                await _myDataRepository.ExportTransactionsAsync(_userContext.UserId, cancellationToken);

            return new ExportDataDto(accounts.ToImmutableArray(),
                categories.ToImmutableArray(),
                cashflows.ToImmutableArray(),
                transactions.ToImmutableArray());
        }
    }
}

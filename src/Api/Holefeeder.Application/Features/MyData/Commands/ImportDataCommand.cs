using System.ComponentModel.DataAnnotations;

using Carter;

using FluentValidation;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.MyData.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Application.SeedWork.BackgroundRequest;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;

namespace Holefeeder.Application.Features.MyData.Commands;

public partial class ImportData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/my-data/import-data", async (Request request, IMediator mediator) =>
            {
                var requestResult = await mediator.Send(request);
                return Results.AcceptedAtRoute(nameof(ImportDataStatus), new {Id = requestResult}, new {Id = requestResult});
            })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ImportData))
            .RequireAuthorization();
    }

    public record Request : IRequest<Guid>
    {
        [Required] public bool UpdateExisting { get; init; }

        [Required] public Dto Data { get; init; } = null!;

        public record Dto
        {
            public MyDataAccountDto[] Accounts { get; init; } = Array.Empty<MyDataAccountDto>();
            public MyDataCategoryDto[] Categories { get; init; } = Array.Empty<MyDataCategoryDto>();
            public MyDataCashflowDto[] Cashflows { get; init; } = Array.Empty<MyDataCashflowDto>();
            public MyDataTransactionDto[] Transactions { get; init; } = Array.Empty<MyDataTransactionDto>();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Data)
                .NotNull()
                .Must(data =>
                    data.Accounts.Any() ||
                    data.Categories.Any() ||
                    data.Cashflows.Any() ||
                    data.Transactions.Any())
                .WithMessage("must contain at least 1 array of accounts|categories|cashflows|transactions");
        }
    }

    public class Handler
        : BackgroundRequestHandler<Request, BackgroundTask, ImportDataStatusDto>
    {
        public Handler(
            IUserContext userContext,
            IServiceProvider serviceProvider,
            BackgroundWorkerQueue backgroundWorkerQueue,
            IMemoryCache memoryCache) : base(serviceProvider, backgroundWorkerQueue, memoryCache)
        {
            UserId = userContext.UserId;
        }
    }
}

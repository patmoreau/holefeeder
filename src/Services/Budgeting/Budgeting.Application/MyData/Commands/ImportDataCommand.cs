using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using ValidationResult = FluentValidation.Results.ValidationResult;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Commands;

public static partial class ImportData
{
    public record Request : IRequest<RequestResponse>, IValidateable
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

    public class Validator : AbstractValidator<Request>, IValidator<Request, RequestResponse>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(command => command.Data)
                .NotNull()
                .Must(data =>
                    data.Accounts.Any() ||
                    data.Categories.Any() ||
                    data.Cashflows.Any() ||
                    data.Transactions.Any())
                .WithMessage("must contain at least 1 array of accounts|categories|cashflows|transactions");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public RequestResponse CreateResponse(ValidationResult result)
        {
            return new(new ValidationErrorsRequestResult(result.ToDictionary()));
        }
    }

    public class Handler
        : BackgroundRequestHandler<Request, BackgroundTask, ImportDataStatusDto>
    {
        public Handler(
            ItemsCache cache,
            IServiceProvider serviceProvider,
            BackgroundWorkerQueue backgroundWorkerQueue,
            IMemoryCache memoryCache) : base(serviceProvider, backgroundWorkerQueue, memoryCache)
        {
            UserId = (Guid)cache["UserId"];
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
    public record Request([Required] bool UpdateExisting, [Required] JsonDocument Data)
        : IRequest<RequestResponse>, IValidateable;

    public class Validator : AbstractValidator<Request>, IValidator<Request, RequestResponse>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(command => command.Data)
                .NotNull()
                .Must(data =>
                    data.RootElement.TryGetProperty("accounts", out _) ||
                    data.RootElement.TryGetProperty("categories", out _) ||
                    data.RootElement.TryGetProperty("cashflows", out _) ||
                    data.RootElement.TryGetProperty("transactions", out _))
                .WithMessage("must contain at least 1 array of accounts|categories|cashflows|transactions");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public RequestResponse CreateResponse(ValidationResult result) =>
            new(new ValidationErrorsRequestResult(result.ToDictionary()));
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

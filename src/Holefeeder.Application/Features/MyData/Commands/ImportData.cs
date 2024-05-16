using System.ComponentModel.DataAnnotations;

using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.MyData.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using ValidationException = FluentValidation.ValidationException;

namespace Holefeeder.Application.Features.MyData.Commands;

public class ImportData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/my-data/import-data",
                [DisableRequestSizeLimit] (Request request, IUserContext userContext, IValidator<Request> validator,
                    IRequestScheduler requestScheduler) =>
                {
                    var validation = validator.Validate(request);
                    if (!validation.IsValid)
                    {
                        throw new ValidationException(validation.Errors);
                    }

                    InternalRequest internalRequest = new()
                    {
                        RequestId = Guid.NewGuid(),
                        UpdateExisting = request.UpdateExisting,
                        Data = request.Data,
                        UserId = userContext.Id
                    };
                    requestScheduler.SendNow(internalRequest, nameof(ImportData));

                    return Results.AcceptedAtRoute(nameof(ImportDataStatus), new { Id = internalRequest.RequestId },
                        new { Id = internalRequest.RequestId });
                })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ImportData))
            .RequireAuthorization();

    internal class Handler(BudgetingContext context, IMemoryCache memoryCache, ILogger<Handler> logger) : IRequestHandler<InternalRequest, Unit>
    {
        private ImportDataStatusDto _importDataStatus = ImportDataStatusDto.Init();

        public async Task<Unit> Handle(InternalRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await context.BeginWorkAsync(cancellationToken);

                UpdateProgress(request.RequestId, _importDataStatus with { Status = CommandStatus.InProgress });
                await ImportAccountsAsync(request, cancellationToken);
                await ImportCategoriesAsync(request, cancellationToken);
                await ImportCashflowsAsync(request, cancellationToken);
                await ImportTransactionsAsync(request, cancellationToken);

                await context.CommitWorkAsync(cancellationToken);

                UpdateProgress(request.RequestId, _importDataStatus with { Status = CommandStatus.Completed });
            }
#pragma warning disable CA1031
            catch (Exception e)
            {
                UpdateProgress(request.RequestId,
                    _importDataStatus with { Status = CommandStatus.Error, Message = e.ToString() });
                await context.RollbackWorkAsync(cancellationToken);
            }
#pragma warning restore CA1031

            return Unit.Value;
        }

        private void UpdateProgress(Guid requestId, ImportDataStatusDto response) =>
            memoryCache.Set(requestId, response, TimeSpan.FromHours(1));

        private async Task ImportAccountsAsync(InternalRequest request, CancellationToken cancellationToken)
        {
            if (request.Data.Accounts.Length == 0)
            {
                return;
            }

            MyDataAccountDto[] accounts = request.Data.Accounts;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                AccountsTotal = accounts.Length
            };
            UpdateProgress(request.RequestId, _importDataStatus);

            foreach (var element in accounts)
            {
                var exists = await context.Accounts
                    .SingleOrDefaultAsync(account => account.Id == element.Id && account.UserId == request.UserId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        exists = exists with
                        {
                            Type = element.Type,
                            Name = element.Name,
                            Favorite = element.Favorite,
                            OpenBalance = element.OpenBalance,
                            OpenDate = element.OpenDate,
                            Description = element.Description,
                            Inactive = element.Inactive
                        };
                        logger.LogAccount("Modify", exists);
                        context.Update(exists);
                    }
                    else
                    {
                        logger.LogAccount("Ignore", exists);
                    }
                }
                else
                {
                    Account account =
                        new()
                        {
                            Id = element.Id,
                            Type = element.Type,
                            Name = element.Name,
                            OpenDate = element.OpenDate,
                            UserId = request.UserId,
                            Favorite = element.Favorite,
                            OpenBalance = element.OpenBalance,
                            Description = element.Description,
                            Inactive = element.Inactive
                        };
                    logger.LogAccount("Create", account);
                    await context.Accounts.AddAsync(account, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    AccountsProcessed = _importDataStatus.AccountsProcessed + 1
                };
                UpdateProgress(request.RequestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task ImportCategoriesAsync(InternalRequest request, CancellationToken cancellationToken)
        {
            if (request.Data.Categories.Length == 0)
            {
                return;
            }

            MyDataCategoryDto[] categories = request.Data.Categories;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                CategoriesTotal = categories.Length
            };
            UpdateProgress(request.RequestId, _importDataStatus);

            foreach (var element in categories)
            {
                var exists = await context.Categories
                    .SingleOrDefaultAsync(category => category.Id == element.Id && category.UserId == request.UserId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        exists = exists with
                        {
                            Type = element.Type,
                            Name = element.Name,
                            Favorite = element.Favorite,
                            System = element.System,
                            BudgetAmount = element.BudgetAmount,
                            Color = element.Color
                        };
                        logger.LogCategory("Modify", exists);
                        context.Update(exists);
                    }
                    else
                    {
                        logger.LogCategory("Ignore", exists);
                    }
                }
                else
                {
                    Category category = new()
                    {
                        Id = element.Id,
                        Type = element.Type,
                        Name = element.Name,
                        UserId = request.UserId,
                        Favorite = element.Favorite,
                        System = element.System,
                        BudgetAmount = element.BudgetAmount,
                        Color = element.Color
                    };
                    logger.LogCategory("Create", category);
                    await context.Categories.AddAsync(category, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    CategoriesProcessed = _importDataStatus.CategoriesProcessed + 1
                };
                UpdateProgress(request.RequestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task ImportCashflowsAsync(InternalRequest request, CancellationToken cancellationToken)
        {
            if (request.Data.Cashflows.Length == 0)
            {
                return;
            }

            MyDataCashflowDto[] cashflows = request.Data.Cashflows;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                CashflowsTotal = cashflows.Length
            };
            UpdateProgress(request.RequestId, _importDataStatus);

            foreach (var element in cashflows)
            {
                var exists = await context.Cashflows
                    .SingleOrDefaultAsync(cashflow => cashflow.Id == element.Id && cashflow.UserId == request.UserId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        exists = exists with
                        {
                            EffectiveDate = element.EffectiveDate,
                            IntervalType = element.IntervalType,
                            Frequency = element.Frequency,
                            Recurrence = element.Recurrence,
                            Amount = element.Amount,
                            Description = element.Description,
                            CategoryId = element.CategoryId,
                            AccountId = element.AccountId,
                            Inactive = element.Inactive
                        };
                        exists = exists.SetTags(element.Tags);
                        logger.LogCashflow("Modify", exists);
                        context.Update(exists);
                    }
                    else
                    {
                        logger.LogCashflow("Ignore", exists);
                    }
                }
                else
                {
                    var cashflow = new Cashflow
                    {
                        Id = element.Id,
                        EffectiveDate = element.EffectiveDate,
                        Amount = element.Amount,
                        UserId = request.UserId,
                        IntervalType = element.IntervalType,
                        Frequency = element.Frequency,
                        Recurrence = element.Recurrence,
                        Description = element.Description,
                        CategoryId = element.CategoryId,
                        AccountId = element.AccountId,
                        Inactive = element.Inactive
                    };
                    cashflow = cashflow.SetTags(element.Tags);
                    logger.LogCashflow("Create", cashflow);
                    await context.Cashflows.AddAsync(cashflow, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    CashflowsProcessed = _importDataStatus.CashflowsProcessed + 1
                };
                UpdateProgress(request.RequestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
        }


        private async Task ImportTransactionsAsync(InternalRequest request, CancellationToken cancellationToken)
        {
            if (request.Data.Transactions.Length == 0)
            {
                return;
            }

            MyDataTransactionDto[] transactions = request.Data.Transactions;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                TransactionsTotal = transactions.Length
            };
            UpdateProgress(request.RequestId, _importDataStatus);

            foreach (var element in transactions)
            {
                var exists = await context.Transactions
                    .SingleOrDefaultAsync(
                        transaction => transaction.Id == element.Id && transaction.UserId == request.UserId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        exists = exists with
                        {
                            Date = element.Date,
                            Amount = element.Amount,
                            Description = element.Description,
                            CategoryId = element.CategoryId,
                            AccountId = element.AccountId,
                            UserId = request.UserId
                        };
                        exists = exists.SetTags(element.Tags);
                        logger.LogTransaction("Modify", exists);
                        context.Update(exists);
                    }
                    else
                    {
                        logger.LogTransaction("Ignore", exists);
                    }
                }
                else
                {
                    var transaction = Transaction.Create(element.Id,
                        element.Date,
                        element.Amount,
                        element.Description,
                        element.AccountId,
                        element.CategoryId,
                        request.UserId);

                    if (element.CashflowId is not null)
                    {
                        transaction = transaction.ApplyCashflow(element.CashflowId.GetValueOrDefault(),
                            element.CashflowDate.GetValueOrDefault());
                    }

                    transaction = transaction.SetTags(element.Tags);
                    logger.LogTransaction("Create", transaction);
                    await context.Transactions.AddAsync(transaction, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    TransactionsProcessed = _importDataStatus.TransactionsProcessed + 1
                };
                UpdateProgress(request.RequestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() =>
            RuleFor(command => command.Data)
                .NotNull()
                .Must(data =>
                    data.Accounts.Length > 0 ||
                    data.Categories.Length > 0 ||
                    data.Cashflows.Length > 0 ||
                    data.Transactions.Length > 0)
                .WithMessage("must contain at least 1 array of accounts|categories|cashflows|transactions");
    }

    internal record Request : IRequest<Unit>
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

    internal record InternalRequest : Request
    {
        public Guid RequestId { get; init; }

        public Guid UserId { get; init; }
    }
}

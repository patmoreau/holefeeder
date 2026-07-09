using System.ComponentModel.DataAnnotations;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.MyData.Exceptions;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.MyData.Queries;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.MyData.Commands;

public class ImportData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/my-data/import-data",
                [DisableRequestSizeLimit](Request request, IUserContext userContext,
                    IRequestScheduler requestScheduler, BudgetingContext context, IMemoryCache memoryCache, ILogger<Handler> logger) =>
                {
                    var requestId = Guid.NewGuid();
                    var userId = userContext.Id;

                    requestScheduler.QueueHandler<Handler>(handler => handler.Handle(
                        requestId,
                        request,
                        userId.Value,
                        CancellationToken.None), nameof(ImportData));

                    return Results.AcceptedAtRoute(nameof(ImportDataStatus), new {Id = requestId}, new {Id = requestId});
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ImportData))
            .RequireAuthorization(Policies.WriteUser);

    internal class Handler(BudgetingContext context, IMemoryCache memoryCache, ILogger<Handler> logger)
    {
        private ImportDataStatusDto _importDataStatus = ImportDataStatusDto.Init();

        public async Task Handle(Guid requestId, Request request, Guid uId, CancellationToken cancellationToken)
        {
            try
            {
                UserId userId = UserId.Create(uId);
                await context.BeginWorkAsync(cancellationToken);

                UpdateProgress(requestId, _importDataStatus with {Status = CommandStatus.InProgress});
                ThrowImportExceptionOnFailure(await ImportAccountsAsync(requestId, request, userId, cancellationToken));
                ThrowImportExceptionOnFailure(await ImportCategoriesAsync(requestId, request, userId, cancellationToken));
                ThrowImportExceptionOnFailure(await ImportCashflowsAsync(requestId, request, userId, cancellationToken));
                ThrowImportExceptionOnFailure(await ImportTransactionsAsync(requestId, request, userId, cancellationToken));

                await context.CommitWorkAsync(cancellationToken);

                UpdateProgress(requestId, _importDataStatus with {Status = CommandStatus.Completed});
            }
#pragma warning disable CA1031
            catch (Exception e)
            {
                UpdateProgress(requestId,
                    _importDataStatus with {Status = CommandStatus.Error, Message = e.ToString()});
                await context.RollbackWorkAsync(cancellationToken);
            }
#pragma warning restore CA1031

            return;

            void ThrowImportExceptionOnFailure(Result<Nothing> result)
            {
                if (result.IsFailure)
                {
                    throw new ImportException(result.Error);
                }
            }
        }

        private void UpdateProgress(Guid requestId, ImportDataStatusDto response) =>
            memoryCache.Set(requestId, response, TimeSpan.FromHours(1));

        private async Task<Result<Nothing>> ImportAccountsAsync(Guid requestId, Request request, UserId userId, CancellationToken cancellationToken)
        {
            if (request.Data.Accounts.Length == 0)
            {
                return Nothing.Value;
            }

            var accounts = request.Data.Accounts;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                AccountsTotal = accounts.Length
            };
            UpdateProgress(requestId, _importDataStatus);

            foreach (var element in accounts)
            {
                var elementId = AccountId.Create(element.Id);
                var elementOpenBalance = Money.Create(element.OpenBalance);
                if (elementOpenBalance.IsFailure)
                {
                    return elementOpenBalance.Error;
                }

                var exists = await context.Accounts
                    .SingleOrDefaultAsync(account => account.Id == elementId && account.UserId == userId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        var result = exists.Modify(
                            element.Type,
                            element.Name,
                            elementOpenBalance.Value,
                            element.OpenDate,
                            element.Description,
                            element.Favorite,
                            element.Inactive
                        );
                        if (result.IsFailure)
                        {
                            return result.Error;
                        }

                        logger.LogAccount("Modify", result.Value);
                        context.Update(result.Value);
                    }
                    else
                    {
                        logger.LogAccount("Ignore", exists);
                    }
                }
                else
                {
                    var result = Account.Import(
                        elementId,
                        element.Type,
                        element.Name,
                        elementOpenBalance.Value,
                        element.OpenDate,
                        element.Description,
                        element.Favorite,
                        element.Inactive,
                        userId
                    );
                    if (result.IsFailure)
                    {
                        return result.Error;
                    }

                    logger.LogAccount("Create", result.Value);
                    await context.Accounts.AddAsync(result.Value, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    AccountsProcessed = _importDataStatus.AccountsProcessed + 1
                };
                UpdateProgress(requestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Nothing.Value;
        }

        private async Task<Result<Nothing>> ImportCategoriesAsync(Guid requestId, Request request, UserId userId, CancellationToken cancellationToken)
        {
            if (request.Data.Categories.Length == 0)
            {
                return Nothing.Value;
            }

            var categories = request.Data.Categories;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                CategoriesTotal = categories.Length
            };
            UpdateProgress(requestId, _importDataStatus);

            foreach (var element in categories)
            {
                var elementId = CategoryId.Create(element.Id);
                var elementBudgetAmount = Money.Create(element.BudgetAmount);
                if (elementBudgetAmount.IsFailure)
                {
                    return elementBudgetAmount.Error;
                }

                var elementColor = CategoryColor.Create(element.Color);
                if (elementColor.IsFailure)
                {
                    return elementColor.Error;
                }

                var exists = await context.Categories
                    .SingleOrDefaultAsync(category => category.Id == elementId && category.UserId == userId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        var result = exists.Modify(
                            element.Type,
                            element.Name,
                            elementColor.Value,
                            element.Favorite,
                            element.System,
                            elementBudgetAmount.Value
                        );
                        if (result.IsFailure)
                        {
                            return result.Error;
                        }

                        logger.LogCategory("Modify", result.Value);
                        context.Update(result.Value);
                    }
                    else
                    {
                        logger.LogCategory("Ignore", exists);
                    }
                }
                else
                {
                    var result = Category.Import(
                        elementId,
                        element.Type,
                        element.Name,
                        elementColor.Value,
                        element.Favorite,
                        element.System,
                        elementBudgetAmount.Value,
                        userId);

                    if (result.IsFailure)
                    {
                        return result.Error;
                    }

                    logger.LogCategory("Create", result.Value);
                    await context.Categories.AddAsync(result.Value, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    CategoriesProcessed = _importDataStatus.CategoriesProcessed + 1
                };
                UpdateProgress(requestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Nothing.Value;
        }

        private async Task<Result<Nothing>> ImportCashflowsAsync(Guid requestId, Request request, UserId userId, CancellationToken cancellationToken)
        {
            if (request.Data.Cashflows.Length == 0)
            {
                return Nothing.Value;
            }

            var cashflows = request.Data.Cashflows;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                CashflowsTotal = cashflows.Length
            };
            UpdateProgress(requestId, _importDataStatus);

            foreach (var element in cashflows)
            {
                var elementId = CashflowId.Create(element.Id);
                var elementAmount = Money.Create(element.Amount);
                if (elementAmount.IsFailure)
                {
                    return elementAmount.Error;
                }

                var elementCategoryId = CategoryId.Create(element.CategoryId);
                var elementAccountId = AccountId.Create(element.AccountId);

                var exists = await context.Cashflows
                    .SingleOrDefaultAsync(cashflow => cashflow.Id == elementId && cashflow.UserId == userId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        var result = exists.Modify(element.EffectiveDate,
                            element.IntervalType,
                            element.Frequency,
                            element.Recurrence,
                            elementAmount.Value,
                            element.Description,
                            elementCategoryId,
                            elementAccountId,
                            element.Inactive
                        );
                        if (result.IsFailure)
                        {
                            return result.Error;
                        }

                        var cashflow = result.Value.SetTags(element.Tags);
                        logger.LogCashflow("Modify", cashflow.Value);
                        context.Update(cashflow.Value);
                    }
                    else
                    {
                        logger.LogCashflow("Ignore", exists);
                    }
                }
                else
                {
                    var result = Cashflow.Import(
                        elementId,
                        element.EffectiveDate,
                        element.IntervalType,
                        element.Frequency,
                        element.Recurrence,
                        elementAmount.Value,
                        element.Description,
                        elementCategoryId,
                        elementAccountId,
                        element.Inactive,
                        userId
                    );
                    if (result.IsFailure)
                    {
                        return result.Error;
                    }

                    var cashflow = result.Value.SetTags(element.Tags);
                    logger.LogCashflow("Create", cashflow.Value);
                    await context.Cashflows.AddAsync(cashflow.Value, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    CashflowsProcessed = _importDataStatus.CashflowsProcessed + 1
                };
                UpdateProgress(requestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Nothing.Value;
        }

        private async Task<Result<Nothing>> ImportTransactionsAsync(Guid requestId, Request request, UserId userId, CancellationToken cancellationToken)
        {
            if (request.Data.Transactions.Length == 0)
            {
                return Nothing.Value;
            }

            var transactions = request.Data.Transactions;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress,
                TransactionsTotal = transactions.Length
            };
            UpdateProgress(requestId, _importDataStatus);

            foreach (var element in transactions)
            {
                var elementId = TransactionId.Create(element.Id);
                var elementAmount = Money.Create(element.Amount);
                if (elementAmount.IsFailure)
                {
                    return elementAmount.Error;
                }

                var elementCategoryId = CategoryId.Create(element.CategoryId);
                var elementAccountId = AccountId.Create(element.AccountId);
                var elementCashflowId =
                    element.CashflowId is not null ? CashflowId.Create(element.CashflowId.Value) : null;

                var exists = await context.Transactions
                    .SingleOrDefaultAsync(
                        transaction => transaction.Id == elementId && transaction.UserId == userId,
                        cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        var result = exists.Modify(
                            element.Date,
                            elementAmount.Value,
                            element.Description,
                            elementAccountId,
                            elementCategoryId,
                            elementCashflowId,
                            element.CashflowDate);

                        if (result.IsFailure)
                        {
                            return result.Error;
                        }

                        var transaction = result.Value.SetTags(element.Tags);
                        logger.LogTransaction("Modify", transaction.Value);
                        context.Update(transaction.Value);
                    }
                    else
                    {
                        logger.LogTransaction("Ignore", exists);
                    }
                }
                else
                {
                    var result = Transaction.Import(elementId,
                        element.Date,
                        elementAmount.Value,
                        element.Description,
                        elementAccountId,
                        elementCategoryId,
                        elementCashflowId,
                        element.CashflowDate,
                        userId);

                    if (result.IsFailure)
                    {
                        return result.Error;
                    }

                    var transaction = result.Value.SetTags(element.Tags);
                    logger.LogTransaction("Create", transaction.Value);
                    await context.Transactions.AddAsync(transaction.Value, cancellationToken);
                }

                _importDataStatus = _importDataStatus with
                {
                    TransactionsProcessed = _importDataStatus.TransactionsProcessed + 1
                };
                UpdateProgress(requestId, _importDataStatus);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Nothing.Value;
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

    internal record Request
    {
        [Required] public bool UpdateExisting { get; init; }

        [Required] public Dto Data { get; init; } = null!;

        public record Dto
        {
            public MyDataAccountDto[] Accounts { get; init; } = [];
            public MyDataCategoryDto[] Categories { get; init; } = [];
            public MyDataCashflowDto[] Cashflows { get; init; } = [];
            public MyDataTransactionDto[] Transactions { get; init; } = [];
        }
    }
}

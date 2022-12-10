using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.MyData.Commands.ImportData;

public class Handler : IRequestHandler<InternalRequest, Unit>
{
    private readonly BudgetingContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<Handler> _logger;

    private ImportDataStatusDto _importDataStatus = ImportDataStatusDto.Init();

    public Handler(BudgetingContext context, IMemoryCache memoryCache, ILogger<Handler> logger)
    {
        _context = context;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<Unit> Handle(InternalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BeginTransactionAsync(cancellationToken);

            UpdateProgress(request.RequestId, _importDataStatus with {Status = CommandStatus.InProgress});
            await ImportAccountsAsync(request, cancellationToken);
            await ImportCategoriesAsync(request, cancellationToken);
            await ImportCashflowsAsync(request, cancellationToken);
            await ImportTransactionsAsync(request, cancellationToken);

            await _context.CommitTransactionAsync(cancellationToken);

            UpdateProgress(request.RequestId, _importDataStatus with {Status = CommandStatus.Completed});
        }
#pragma warning disable CA1031
        catch (Exception e)
        {
            UpdateProgress(request.RequestId,
                _importDataStatus with {Status = CommandStatus.Error, Message = e.ToString()});
            await _context.RollbackTransactionAsync(cancellationToken);
        }
#pragma warning restore CA1031

        return Unit.Value;
    }

    private void UpdateProgress(Guid requestId, ImportDataStatusDto response)
    {
        _memoryCache.Set(requestId, response, TimeSpan.FromHours(1));
    }

    private async Task ImportAccountsAsync(InternalRequest request, CancellationToken cancellationToken)
    {
        if (!request.Data.Accounts.Any())
        {
            return;
        }

        var accounts = request.Data.Accounts;

        _importDataStatus = _importDataStatus with {Status = CommandStatus.InProgress, AccountsTotal = accounts.Length};
        UpdateProgress(request.RequestId, _importDataStatus);

        foreach (var element in accounts)
        {
            var exists = await _context.Accounts
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
                    _logger.LogAccount("Modify", exists);
                    _context.Update(exists);
                }
                else
                {
                    _logger.LogAccount("Ignore", exists);
                }
            }
            else
            {
                var account = new Account(element.Id, element.Type, element.Name, element.OpenDate, request.UserId)
                {
                    Favorite = element.Favorite,
                    OpenBalance = element.OpenBalance,
                    Description = element.Description,
                    Inactive = element.Inactive
                };
                _logger.LogAccount("Create", account);
                await _context.Accounts.AddAsync(account, cancellationToken);
            }

            _importDataStatus = _importDataStatus with {AccountsProcessed = _importDataStatus.AccountsProcessed + 1};
            UpdateProgress(request.RequestId, _importDataStatus);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportCategoriesAsync(InternalRequest request, CancellationToken cancellationToken)
    {
        if (!request.Data.Categories.Any())
        {
            return;
        }

        var categories = request.Data.Categories;

        _importDataStatus = _importDataStatus with
        {
            Status = CommandStatus.InProgress, CategoriesTotal = categories.Length
        };
        UpdateProgress(request.RequestId, _importDataStatus);

        foreach (var element in categories)
        {
            var exists = await _context.Categories
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
                    _logger.LogCategory("Modify", exists);
                    _context.Update(exists);
                }
                else
                {
                    _logger.LogCategory("Ignore", exists);
                }
            }
            else
            {
                var category = new Category(element.Id, element.Type, element.Name, request.UserId)
                {
                    Favorite = element.Favorite,
                    System = element.System,
                    BudgetAmount = element.BudgetAmount,
                    Color = element.Color
                };
                _logger.LogCategory("Create", category);
                await _context.Categories.AddAsync(category, cancellationToken);
            }

            _importDataStatus = _importDataStatus with
            {
                CategoriesProcessed = _importDataStatus.CategoriesProcessed + 1
            };
            UpdateProgress(request.RequestId, _importDataStatus);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportCashflowsAsync(InternalRequest request, CancellationToken cancellationToken)
    {
        if (!request.Data.Cashflows.Any())
        {
            return;
        }

        var cashflows = request.Data.Cashflows;

        _importDataStatus = _importDataStatus with
        {
            Status = CommandStatus.InProgress, CashflowsTotal = cashflows.Length
        };
        UpdateProgress(request.RequestId, _importDataStatus);

        foreach (var element in cashflows)
        {
            var exists = await _context.Cashflows
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
                    _logger.LogCashflow("Modify", exists);
                    _context.Update(exists);
                }
                else
                {
                    _logger.LogCashflow("Ignore", exists);
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
                _logger.LogCashflow("Create", cashflow);
                await _context.Cashflows.AddAsync(cashflow, cancellationToken);
            }

            _importDataStatus = _importDataStatus with {CashflowsProcessed = _importDataStatus.CashflowsProcessed + 1};
            UpdateProgress(request.RequestId, _importDataStatus);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }


    private async Task ImportTransactionsAsync(InternalRequest request, CancellationToken cancellationToken)
    {
        if (!request.Data.Transactions.Any())
        {
            return;
        }

        var transactions = request.Data.Transactions;

        _importDataStatus = _importDataStatus with
        {
            Status = CommandStatus.InProgress, TransactionsTotal = transactions.Length
        };
        UpdateProgress(request.RequestId, _importDataStatus);

        foreach (var element in transactions)
        {
            var exists = await _context.Transactions
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
                    _logger.LogTransaction("Modify", exists);
                    _context.Update(exists);
                }
                else
                {
                    _logger.LogTransaction("Ignore", exists);
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
                _logger.LogTransaction("Create", transaction);
                await _context.Transactions.AddAsync(transaction, cancellationToken);
            }

            _importDataStatus = _importDataStatus with
            {
                TransactionsProcessed = _importDataStatus.TransactionsProcessed + 1
            };
            UpdateProgress(request.RequestId, _importDataStatus);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

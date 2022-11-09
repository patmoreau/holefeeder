using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.SeedWork;
using Holefeeder.Application.SeedWork.BackgroundRequest;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.MyData.Commands;

public partial class ImportData
{
    internal class BackgroundTask : IBackgroundTask<Request, ImportDataStatusDto>
    {
        private readonly IAccountRepository _accountsRepository;

        private readonly ICashflowRepository _cashflowRepository;
        private readonly ILogger<BackgroundTask> _logger;
        private readonly ITransactionRepository _transactionRepository;
        private readonly BudgetingContext _context;

        private ImportDataStatusDto _importDataStatus = ImportDataStatusDto.Init();

        public BackgroundTask(
            IAccountRepository accountsRepository,
            ICashflowRepository cashflowRepository,
            ITransactionRepository transactionRepository,
            BudgetingContext context,
            ILogger<BackgroundTask> logger
        )
        {
            _accountsRepository = accountsRepository;
            _cashflowRepository = cashflowRepository;
            _transactionRepository = transactionRepository;
            _context = context;
            _logger = logger;
        }

        public async Task Handle(Guid userId, Request request, Action<ImportDataStatusDto> updateProgress,
            CancellationToken cancellationToken)
        {
            try
            {
                updateProgress(_importDataStatus with {Status = CommandStatus.InProgress});
                await ImportAccountsAsync(userId, request, updateProgress, cancellationToken);
                await ImportCategoriesAsync(userId, request, updateProgress, cancellationToken);
                await ImportCashflowsAsync(userId, request, updateProgress, cancellationToken);
                await ImportTransactionsAsync(userId, request, updateProgress, cancellationToken);

                await _accountsRepository.UnitOfWork.CommitAsync(cancellationToken);

                updateProgress(_importDataStatus with {Status = CommandStatus.Completed});
            }
#pragma warning disable CA1031
            catch (Exception e)
            {
                updateProgress(_importDataStatus with {Status = CommandStatus.Error, Message = e.Message});
            }
#pragma warning restore CA1031
        }

        private async Task ImportAccountsAsync(Guid userId, Request request, Action<ImportDataStatusDto> updateProgress,
            CancellationToken cancellationToken)
        {
            if (!request.Data.Accounts.Any())
            {
                return;
            }

            var accounts = request.Data.Accounts;

            _importDataStatus = _importDataStatus with
            {
                Status = CommandStatus.InProgress, AccountsTotal = accounts.Length
            };
            updateProgress(_importDataStatus);

            foreach (var element in accounts)
            {
                Account account = null!;

                var exists = await _accountsRepository.FindByIdAsync(element.Id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        account = exists with
                        {
                            Type = element.Type,
                            Name = element.Name,
                            Favorite = element.Favorite,
                            OpenBalance = element.OpenBalance,
                            OpenDate = element.OpenDate,
                            Description = element.Description,
                            Inactive = element.Inactive
                        };
                        _logger.LogAccount("Modify", account);
                    }
                    else
                    {
                        _logger.LogAccount("Ignore", exists);
                    }
                }
                else
                {
                    account = new Account(element.Id, element.Type, element.Name, element.OpenDate, userId)
                    {
                        Favorite = element.Favorite,
                        OpenBalance = element.OpenBalance,
                        Description = element.Description,
                        Inactive = element.Inactive
                    };
                    _logger.LogAccount("Create", account);
                }

                await _accountsRepository.SaveAsync(account, cancellationToken);

                _importDataStatus = _importDataStatus with
                {
                    AccountsProcessed = _importDataStatus.AccountsProcessed + 1
                };
                updateProgress(_importDataStatus);
            }
        }

        private async Task ImportCategoriesAsync(Guid userId, Request request,
            Action<ImportDataStatusDto> updateProgress, CancellationToken cancellationToken)
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
            updateProgress(_importDataStatus);

            foreach (var element in categories)
            {
                Category category = null!;

                var exists = await _context.Categories.AsQueryable()
                    .FirstOrDefaultAsync(x => x.Id == element.Id && x.UserId == userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        category = exists with
                        {
                            Type = element.Type,
                            Name = element.Name,
                            Favorite = element.Favorite,
                            System = element.System,
                            BudgetAmount = element.BudgetAmount,
                            Color = element.Color
                        };
                        _logger.LogCategory("Modify", category);
                    }
                    else
                    {
                        _logger.LogCategory("Ignore", exists);
                    }
                }
                else
                {
                    category = new Category(element.Id, element.Type, element.Name, userId)
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
                updateProgress(_importDataStatus);
            }
        }

        private async Task ImportCashflowsAsync(Guid userId, Request request,
            Action<ImportDataStatusDto> updateProgress, CancellationToken cancellationToken)
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
            updateProgress(_importDataStatus);

            foreach (var element in cashflows)
            {
                Cashflow cashflow = null!;

                var exists = await _cashflowRepository.FindByIdAsync(element.Id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        cashflow = exists with
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
                        cashflow = cashflow.SetTags(element.Tags);
                        _logger.LogCashflow("Modify", cashflow);
                    }
                    else
                    {
                        _logger.LogCashflow("Ignore", exists);
                    }
                }
                else
                {
                    cashflow = new Cashflow
                    {
                        Id = element.Id,
                        EffectiveDate = element.EffectiveDate,
                        Amount = element.Amount,
                        UserId = userId,
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
                }

                await _cashflowRepository.SaveAsync(cashflow, cancellationToken);

                _importDataStatus = _importDataStatus with
                {
                    CashflowsProcessed = _importDataStatus.CashflowsProcessed + 1
                };
                updateProgress(_importDataStatus);
            }
        }


        private async Task ImportTransactionsAsync(Guid userId, Request request,
            Action<ImportDataStatusDto> updateProgress, CancellationToken cancellationToken)
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
            updateProgress(_importDataStatus);

            foreach (var element in transactions)
            {
                Transaction transaction = null!;

                var exists = await _transactionRepository.FindByIdAsync(element.Id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        transaction = exists with
                        {
                            Date = element.Date,
                            Amount = element.Amount,
                            Description = element.Description,
                            CategoryId = element.CategoryId,
                            AccountId = element.AccountId,
                            UserId = userId
                        };
                        transaction = transaction.SetTags(element.Tags);
                        _logger.LogTransaction("Modify", transaction);
                    }
                    else
                    {
                        _logger.LogTransaction("Ignore", exists);
                    }
                }
                else
                {
                    transaction = Transaction.Create(element.Id,
                        element.Date,
                        element.Amount,
                        element.Description,
                        element.AccountId,
                        element.CategoryId,
                        userId);

                    if (element.CashflowId is not null)
                    {
                        transaction = transaction.ApplyCashflow(element.CashflowId.GetValueOrDefault(),
                            element.CashflowDate.GetValueOrDefault());
                    }

                    transaction = transaction.SetTags(element.Tags);
                    _logger.LogTransaction("Create", transaction);
                }

                await _transactionRepository.SaveAsync(transaction, cancellationToken);

                _importDataStatus = _importDataStatus with
                {
                    TransactionsProcessed = _importDataStatus.TransactionsProcessed + 1
                };
                updateProgress(_importDataStatus);
            }
        }
    }
}

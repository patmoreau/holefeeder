using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

using Holefeeder.Domain.Features.Accounts;

using Microsoft.Extensions.Logging;

using Category = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext.Category;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Commands;

public static partial class ImportData
{
    public class BackgroundTask : IBackgroundTask<Request, ImportDataStatusDto>
    {
        private readonly IAccountRepository _accountsRepository;
        private readonly ICashflowRepository _cashflowRepository;
        private readonly ICategoryRepository _categoriesRepository;
        private readonly ILogger<BackgroundTask> _logger;
        private readonly ITransactionRepository _transactionRepository;

        private ImportDataStatusDto _importDataStatus = ImportDataStatusDto.Init();

        public BackgroundTask(
            IAccountRepository accountsRepository,
            ICategoryRepository categoriesRepository,
            ICashflowRepository cashflowRepository,
            ITransactionRepository transactionRepository,
            ILogger<BackgroundTask> logger
        )
        {
            _accountsRepository = accountsRepository;
            _categoriesRepository = categoriesRepository;
            _cashflowRepository = cashflowRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task Handle(Guid userId, Request request,
            Action<ImportDataStatusDto> updateProgress, CancellationToken cancellationToken)
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
            catch (Exception e)
            {
                updateProgress(_importDataStatus with {Status = CommandStatus.Error, Message = e.Message});
            }
        }

        private async Task ImportAccountsAsync(Guid userId, Request request,
            Action<ImportDataStatusDto> updateProgress, CancellationToken cancellationToken)
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
                        _logger.LogInformation("----- Modify Account - Account: {@Account}", account);
                    }
                    else
                    {
                        _logger.LogInformation("----- Ignore Account - Account: {@Account}", exists);
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
                    _logger.LogInformation("----- Create Account - Account: {@Account}", account);
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

                var exists = await _categoriesRepository.FindByIdAsync(element.Id, userId, cancellationToken);
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
                        _logger.LogInformation("----- Modify Category - Category: {@Category}", category);
                    }
                    else
                    {
                        _logger.LogInformation("----- Ignore Category - Category: {@Category}", exists);
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
                    _logger.LogInformation("----- Create Category - Category: {@Category}", category);
                }

                await _categoriesRepository.SaveAsync(category, cancellationToken);

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
                        cashflow = cashflow.AddTags(element.Tags);
                        _logger.LogInformation("----- Modify Cashflow - Cashflow: {@Cashflow}", cashflow);
                    }
                    else
                    {
                        _logger.LogInformation("----- Ignore Cashflow - Cashflow: {@Cashflow}", exists);
                    }
                }
                else
                {
                    cashflow = new Cashflow(element.Id, element.EffectiveDate, element.Amount, userId)
                    {
                        IntervalType = element.IntervalType,
                        Frequency = element.Frequency,
                        Recurrence = element.Recurrence,
                        Amount = element.Amount,
                        Description = element.Description,
                        CategoryId = element.CategoryId,
                        AccountId = element.AccountId,
                        Inactive = element.Inactive
                    };
                    cashflow = cashflow.AddTags(element.Tags);
                    _logger.LogInformation("----- Create Cashflow - Cashflow: {@Cashflow}", cashflow);
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
                            CashflowId = element.CashflowId,
                            CashflowDate = element.CashflowDate,
                            UserId = userId
                        };
                        transaction = transaction.AddTags(element.Tags);
                        _logger.LogInformation("----- Modify Transaction - Transaction: {@Transaction}", transaction);
                    }
                    else
                    {
                        _logger.LogInformation("----- Ignore Transaction - Transaction: {@Transaction}", exists);
                    }
                }
                else
                {
                    transaction = new Transaction
                    {
                        Id = element.Id,
                        Date = element.Date,
                        Amount = element.Amount,
                        Description = element.Description,
                        CategoryId = element.CategoryId,
                        AccountId = element.AccountId,
                        CashflowId = element.CashflowId,
                        CashflowDate = element.CashflowDate,
                        UserId = userId
                    };
                    transaction = transaction.AddTags(element.Tags);
                    _logger.LogInformation("----- Create Transaction - Transaction: {@Transaction}", transaction);
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

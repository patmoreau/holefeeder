using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Microsoft.Extensions.Logging;

using Category = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext.Category;
using Transaction = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext.Transaction;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Commands;

public static partial class ImportData
{
    public class BackgroundTask : IBackgroundTask<Request, ImportDataStatusViewModel>
    {
        private readonly IAccountRepository _accountsRepository;
        private readonly ICategoryRepository _categoriesRepository;
        private readonly ICashflowRepository _cashflowRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<BackgroundTask> _logger;

        private ImportDataStatusViewModel _importDataStatus = ImportDataStatusViewModel.Init();

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
            Action<ImportDataStatusViewModel> updateProgress, CancellationToken cancellationToken)
        {
            try
            {
                updateProgress(_importDataStatus with { Status = CommandStatus.InProgress });
                await ImportAccountsAsync(userId, request, updateProgress, cancellationToken);
                await ImportCategoriesAsync(userId, request, updateProgress, cancellationToken);
                await ImportCashflowsAsync(userId, request, updateProgress, cancellationToken);
                await ImportTransactionsAsync(userId, request, updateProgress, cancellationToken);

                await _accountsRepository.UnitOfWork.CommitAsync(cancellationToken);

                updateProgress(_importDataStatus with { Status = CommandStatus.Completed });
            }
            catch (Exception e)
            {
                updateProgress(_importDataStatus with { Status = CommandStatus.Error, Message = e.Message });
            }
        }

        private async Task ImportAccountsAsync(Guid userId, Request request,
            Action<ImportDataStatusViewModel> updateProgress, CancellationToken cancellationToken)
        {
            if (!ContainsElement(request.Data.RootElement, "accounts"))
            {
                return;
            }

            var accounts = GetElement(request.Data.RootElement, "accounts");

            _importDataStatus = _importDataStatus with { AccountsTotal = accounts.GetArrayLength() };
            updateProgress(_importDataStatus with { Status = CommandStatus.InProgress });

            foreach (var element in accounts.EnumerateArray())
            {
                Account account = null!;
                var id = ConvertToValue<Guid>(element, "id");
                var type = Enumeration.FromName<AccountType>(ConvertToValue<string>(element, "type"));
                var name = ConvertToValue<string>(element, "name");
                var favorite = ConvertToValue(element, "favorite", false);
                var openBalance = ConvertToValue<decimal>(element, "openBalance");
                var openDate = ConvertToValue<DateTime>(element, "openDate");
                var description = ConvertToValue(element, "description", string.Empty);
                var inactive = ConvertToValue(element, "inactive", false);

                var exists = await _accountsRepository.FindByIdAsync(id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        account = exists with
                        {
                            Type = type,
                            Name = name,
                            Favorite = favorite,
                            OpenBalance = openBalance,
                            OpenDate = openDate,
                            Description = description,
                            Inactive = inactive
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
                    account = new Account(id, type, name, openDate, userId)
                    {
                        Favorite = favorite,
                        OpenBalance = openBalance,
                        Description = description,
                        Inactive = inactive
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
            Action<ImportDataStatusViewModel> updateProgress, CancellationToken cancellationToken)
        {
            if (!ContainsElement(request.Data.RootElement, "categories"))
            {
                return;
            }

            var categories = GetElement(request.Data.RootElement, "categories");

            _importDataStatus = _importDataStatus with { CategoriesTotal = categories.GetArrayLength() };
            updateProgress(_importDataStatus);

            foreach (var element in categories.EnumerateArray())
            {
                Category category = null!;
                var id = ConvertToValue<Guid>(element, "id");
                var type = Enumeration.FromName<CategoryType>(ConvertToValue<string>(element, "type"));
                var name = ConvertToValue<string>(element, "name");
                var favorite = ConvertToValue(element, "favorite", false);
                var system = ConvertToValue(element, "system", false);
                var color = ConvertToValue(element, "color", string.Empty);
                var budgetAmount = ConvertToValue<decimal>(element, "budgetAmount");

                var exists = await _categoriesRepository.FindByIdAsync(id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        category = exists with
                        {
                            Type = type,
                            Name = name,
                            Favorite = favorite,
                            System = system,
                            BudgetAmount = budgetAmount,
                            Color = color,
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
                    category = new Category(id, type, name, userId)
                    {
                        Favorite = favorite, System = system, BudgetAmount = budgetAmount, Color = color,
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
            Action<ImportDataStatusViewModel> updateProgress, CancellationToken cancellationToken)
        {
            if (!ContainsElement(request.Data.RootElement, "cashflows"))
            {
                return;
            }

            var cashflows = GetElement(request.Data.RootElement, "cashflows");

            _importDataStatus = _importDataStatus with { CashflowsTotal = cashflows.GetArrayLength() };
            updateProgress(_importDataStatus);

            foreach (var element in cashflows.EnumerateArray())
            {
                Cashflow cashflow = null!;
                var id = ConvertToValue<Guid>(element, "id");
                var effectiveDate = ConvertToValue<DateTime>(element, "effectiveDate");
                var intervalType =
                    Enumeration.FromName<DateIntervalType>(ConvertToValue<string>(element, "intervalType"));
                var frequency = ConvertToValue<int>(element, "frequency");
                var recurrence = ConvertToValue<int>(element, "recurrence");
                var amount = ConvertToValue<decimal>(element, "amount");
                var description = ConvertToValue(element, "description", string.Empty);
                var categoryId = ConvertToValue<Guid>(element, "categoryId");
                var accountId = ConvertToValue<Guid>(element, "accountId");
                var inactive = ConvertToValue(element, "inactive", false);
                var tags = ConvertToStringArray(element, "tags");

                var exists = await _cashflowRepository.FindByIdAsync(id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        cashflow = exists with
                        {
                            EffectiveDate = effectiveDate,
                            IntervalType = intervalType,
                            Frequency = frequency,
                            Recurrence = recurrence,
                            Amount = amount,
                            Description = description,
                            CategoryId = categoryId,
                            AccountId = accountId,
                            UserId = userId,
                            Inactive = inactive
                        };
                        cashflow = cashflow.AddTags(tags);
                        _logger.LogInformation("----- Modify Cashflow - Cashflow: {@Cashflow}", cashflow);
                    }
                    else
                    {
                        _logger.LogInformation("----- Ignore Cashflow - Cashflow: {@Cashflow}", exists);
                    }
                }
                else
                {
                    cashflow = new Cashflow(id, effectiveDate, amount, userId)
                    {
                        IntervalType = intervalType,
                        Frequency = frequency,
                        Recurrence = recurrence,
                        Amount = amount,
                        Description = description,
                        CategoryId = categoryId,
                        AccountId = accountId,
                        Inactive = inactive
                    };
                    cashflow = cashflow.AddTags(tags);
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
            Action<ImportDataStatusViewModel> updateProgress, CancellationToken cancellationToken)
        {
            if (!ContainsElement(request.Data.RootElement, "transactions"))
            {
                return;
            }

            var transactions = GetElement(request.Data.RootElement, "transactions");

            _importDataStatus = _importDataStatus with { TransactionsTotal = transactions.GetArrayLength() };
            updateProgress(_importDataStatus);

            foreach (var element in transactions.EnumerateArray())
            {
                Transaction transaction = null!;
                var id = ConvertToValue<Guid>(element, "id");
                var date = ConvertToValue<DateTime>(element, "date");
                var amount = ConvertToValue<decimal>(element, "amount");
                var description = ConvertToValue(element, "description", string.Empty);
                var categoryId = ConvertToValue<Guid>(element, "categoryId");
                var accountId = ConvertToValue<Guid>(element, "accountId");
                Guid? cashflowId = ConvertToValueOrNull<Guid>(element, "cashflowId");
                DateTime? cashflowDate = ConvertToValueOrNull<DateTime>(element, "cashflowDate");

                var tags = ConvertToStringArray(element, "tags");

                var exists = await _transactionRepository.FindByIdAsync(id, userId, cancellationToken);
                if (exists is not null)
                {
                    if (request.UpdateExisting)
                    {
                        transaction = exists with
                        {
                            Date = date,
                            Amount = amount,
                            Description = description,
                            CategoryId = categoryId,
                            AccountId = accountId,
                            CashflowId = cashflowId,
                            CashflowDate = cashflowDate,
                            UserId = userId
                        };
                        transaction = transaction.AddTags(tags);
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
                        Id = id,
                        Date = date,
                        Amount = amount,
                        Description = description,
                        CategoryId = categoryId,
                        AccountId = accountId,
                        CashflowId = cashflowId,
                        CashflowDate = cashflowDate,
                        UserId = userId
                    };
                    transaction = transaction.AddTags(tags);
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

        private static bool ContainsElement(JsonElement element, string name) =>
            element.EnumerateObject()
                .Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        private static JsonElement GetElement(JsonElement element, string name) =>
            element.EnumerateObject()
                .Single(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Value;

        private static T? ConvertToValueOrNull<T>(JsonElement root, string name) where T : struct
        {
            var element = GetElement(root, name);

            return element.ValueKind == JsonValueKind.Null ? null : ConvertTo<T>(element);
        }

        private static T ConvertToValue<T>(JsonElement root, string name)
        {
            var element = GetElement(root, name);

            if (element.ValueKind == JsonValueKind.Null)
            {
                throw new InvalidOperationException($"'{name}' is null for {root.ToString()}");
            }

            return ConvertTo<T>(element);
        }

        private static T ConvertToValue<T>(JsonElement root, string name, T defaultValueIfNull)
        {
            var element = GetElement(root, name);

            return element.ValueKind == JsonValueKind.Null ? defaultValueIfNull : (T)ConvertTo<T>(element);
        }

        private static string[] ConvertToStringArray(JsonElement root, string name)
        {
            var element = GetElement(root, name);

            return (element.ValueKind is JsonValueKind.Null or not JsonValueKind.Array
                ? Array.Empty<string>()
                : element.EnumerateArray()
                    .Select(s => s.GetString())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray())!;
        }

        private static dynamic ConvertTo<T>(JsonElement element)
        {
            if (typeof(T) == typeof(int))
            {
                return element.GetInt32();
            }

            if (typeof(T) == typeof(Guid))
            {
                return element.GetGuid();
            }

            if (typeof(T) == typeof(bool))
            {
                return element.GetBoolean();
            }

            if (typeof(T) == typeof(DateTime))
            {
                return element.GetDateTime();
            }

            if (typeof(T) == typeof(decimal))
            {
                return element.GetDecimal();
            }

            return element.GetString() ?? string.Empty;
        }
    }
}

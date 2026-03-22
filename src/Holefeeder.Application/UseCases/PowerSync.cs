using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.UseCases;

public partial class PowerSync : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/sync/powersync",
                async (Request request, IUserContext userContext, BudgetingContext context, ILogger<PowerSync> logger, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, logger, cancellationToken);
                    LogPowersyncCompletedWithResultResult(logger, result);
                    return result switch
                    {
                        {IsFailure: true} => result.Error.ToProblem(),
                        _ => Results.NoContent()
                    };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(PowerSync))
            .WithName(nameof(PowerSync))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<Nothing>> Handle(Request request, IUserContext userContext, BudgetingContext context, ILogger logger, CancellationToken cancellationToken)
    {
        if (request.Operations.Count == 0)
            return Nothing.Value;

        await context.BeginWorkAsync(cancellationToken);

        try
        {
            LogStartingOperationsOperationsForPowerSyncForUserUserid(logger, request.Operations.Count, userContext.Id);

            foreach (var op in request.Operations)
            {
                Result<Nothing> result = op.Type switch
                {
                    "store_items" => await ProcessStoreItemsOperation(op, userContext, context, logger, cancellationToken),
                    "transactions" => await ProcessTransactionOperation(op, userContext, context, logger, cancellationToken),
                    "cashflows" => await ProcessCashflowOperation(op, userContext, context, logger, cancellationToken),
                    _ => SyncErrors.TypeInvalid(op.Type)
                };

                if (result.IsSuccess) continue;

                await context.RollbackWorkAsync(cancellationToken);
                return result.Error;
            }

            await context.SaveChangesAsync(cancellationToken);
            await context.CommitWorkAsync(cancellationToken);
            return Nothing.Value;
        }
        catch
        {
            await context.RollbackWorkAsync(cancellationToken);
            throw;
        }
    }

    private static async Task<Result<Nothing>> ProcessStoreItemsOperation(SyncOperation op, IUserContext userContext, BudgetingContext context, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            var storeItemId = StoreItemId.Create(op.Id);
            var exists = await context.StoreItems.SingleOrDefaultAsync(x => x.Id == storeItemId && x.UserId == userContext.Id, cancellationToken);

            if (op.Op == "DELETE")
            {
                if (exists is null)
                {
                    return StoreItemErrors.NotFound(storeItemId);
                }

                context.Remove(exists);
                return Nothing.Value;
            }

            if (exists is null && op.Op == "PATCH")
            {
                return StoreItemErrors.NotFound(storeItemId);
            }

            var code = op.Data.ContainsKey("code") ? ExtractString(op.Data, "code") : null;
            var data = op.Data.ContainsKey("data") ? ExtractString(op.Data, "data") : null;

            if (exists is not null)
            {
                var result = exists.Modify(code, data);
                if (result.IsFailure)
                {
                    return result.Error;
                }

                context.Update(result.Value);
            }
            else
            {
                var result = StoreItem.Import(storeItemId, code ?? string.Empty, data ?? string.Empty, userContext.Id);
                if (result.IsFailure)
                {
                    return result.Error;
                }

                context.StoreItems.Add(result.Value);
            }

            return Nothing.Value;
        }
        catch (Exception)
        {
            LogErrorProcessingStoreItemOperationWithIdIdForUserUserid(logger, op.Id, userContext.Id);
            throw;
        }
    }

    private static async Task<Result<Nothing>> ProcessTransactionOperation(SyncOperation op, IUserContext userContext, BudgetingContext context, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            var transactionId = TransactionId.Create(op.Id);
            var exists = await context.Transactions.SingleOrDefaultAsync(x => x.Id == transactionId && x.UserId == userContext.Id, cancellationToken);

            if (op.Op == "DELETE")
            {
                if (exists is null)
                {
                    return TransactionErrors.NotFound(transactionId);
                }

                context.Remove(exists);
                return Nothing.Value;
            }

            if (exists is null && op.Op == "PATCH")
            {
                return TransactionErrors.NotFound(transactionId);
            }

            var date = op.Data.ContainsKey("date") ? ExtractDate(op.Data, "date") : (DateOnly?) null;
            var amount = op.Data.ContainsKey("amount") ? ExtractMoney(op.Data, "amount") : (Money?) null;
            var description = op.Data.ContainsKey("description") ? ExtractString(op.Data, "description") : null;
            var accountId = op.Data.ContainsKey("account_id") ? ExtractAccountId(op.Data, "account_id") : null;
            var categoryId = op.Data.ContainsKey("category_id") ? ExtractCategoryId(op.Data, "category_id") : null;
            var cashflowId = op.Data.ContainsKey("cashflow_id") ? ExtractCashflowId(op.Data, "cashflow_id") : null;
            var cashflowDate = op.Data.ContainsKey("cashflow_date") ? ExtractNullableDate(op.Data, "cashflow_date") : null;
            var tags = op.Data.ContainsKey("tags") ? ExtractTags(op.Data, "tags") : [];

            if (exists is not null)
            {
                var result = exists.Modify(
                    date,
                    amount,
                    description,
                    accountId,
                    categoryId,
                    cashflowId,
                    cashflowDate);

                if (result.IsFailure)
                {
                    return result.Error;
                }

                var transaction = tags.Length > 0 ? result.Value.SetTags(tags) : result;
                context.Update(transaction.Value);
            }
            else
            {
                var result = Transaction.Import(
                    transactionId,
                    date ?? default,
                    amount ?? default,
                    description ?? string.Empty,
                    accountId ?? AccountId.Empty,
                    categoryId ?? CategoryId.Empty,
                    cashflowId,
                    cashflowDate,
                    userContext.Id);

                var transaction = tags.Length > 0 ? result.Value.SetTags(tags) : result;
                context.Transactions.Add(transaction);
            }

            return Nothing.Value;
        }
        catch (Exception)
        {
            LogErrorProcessingTransactionOperationWithIdIdForUserUserid(logger, op.Id, userContext.Id);
            throw;
        }
    }

    private static async Task<Result<Nothing>> ProcessCashflowOperation(SyncOperation op, IUserContext userContext, BudgetingContext context, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            var cashflowId = CashflowId.Create(op.Id);
            var exists = await context.Cashflows.SingleOrDefaultAsync(x => x.Id == cashflowId && x.UserId == userContext.Id, cancellationToken);

            if (op.Op == "DELETE")
            {
                if (exists is null)
                {
                    return CashflowErrors.NotFound(cashflowId);
                }

                context.Remove(exists);
                return Nothing.Value;
            }

            if (exists is null && op.Op == "PATCH")
            {
                return CashflowErrors.NotFound(cashflowId);
            }

            var effectiveDate = op.Data.ContainsKey("effective_date") ? ExtractDate(op.Data, "effective_date") : (DateOnly?) null;
            var intervalType = op.Data.ContainsKey("interval_type") ? ExtractDateIntervalType(op.Data, "interval_type") : null;
            var frequency = op.Data.ContainsKey("frequency") ? ExtractInt(op.Data, "frequency") : (int?) null;
            var recurrence = op.Data.ContainsKey("recurrence") ? ExtractInt(op.Data, "recurrence") : (int?) null;
            var amount = op.Data.ContainsKey("amount") ? ExtractMoney(op.Data, "amount") : (Money?) null;
            var description = op.Data.ContainsKey("description") ? ExtractString(op.Data, "description") : null;
            var accountId = op.Data.ContainsKey("account_id") ? ExtractAccountId(op.Data, "account_id") : null;
            var categoryId = op.Data.ContainsKey("category_id") ? ExtractCategoryId(op.Data, "category_id") : null;
            var inactive = op.Data.ContainsKey("inactive") ? ExtractInt(op.Data, "inactive") != 0 : (bool?) null;
            var tags = op.Data.ContainsKey("tags") ? ExtractTags(op.Data, "tags") : [];

            if (exists is not null)
            {
                var result = exists.Modify(
                    effectiveDate,
                    intervalType,
                    frequency,
                    recurrence,
                    amount,
                    description,
                    categoryId,
                    accountId,
                    inactive);

                if (result.IsFailure)
                {
                    return result.Error;
                }

                var cashflow = tags.Length > 0 ? result.Value.SetTags(tags) : result;
                context.Update(cashflow.Value);
            }
            else
            {
                var result = Cashflow.Import(
                    cashflowId,
                    effectiveDate ?? default,
                    intervalType ?? DateIntervalType.Monthly,
                    frequency ?? 1,
                    recurrence ?? 0,
                    amount ?? default,
                    description ?? string.Empty,
                    categoryId ?? CategoryId.Empty,
                    accountId ?? AccountId.Empty,
                    inactive ?? false,
                    userContext.Id);

                var cashflow = tags.Length > 0 ? result.Value.SetTags(tags) : result;
                context.Cashflows.Add(cashflow);
            }

            return Nothing.Value;
        }
        catch (Exception)
        {
            LogErrorProcessingCashflowOperationWithIdIdForUserUserid(logger, op.Id, userContext.Id);
            throw;
        }
    }

    private static JsonElement GetJsonElement(IReadOnlyDictionary<string, object> data, string key)
    {
        if (!data.TryGetValue(key, out var value))
        {
            throw new InvalidOperationException($"Required field '{key}' not found in data");
        }

        return value is JsonElement json
            ? json
            : throw new InvalidOperationException($"Field '{key}' is not a JsonElement (got {value?.GetType().Name ?? "null"})");
    }

    private static JsonElement? TryGetJsonElement(IReadOnlyDictionary<string, object> data, string key) => data.TryGetValue(key, out var value) && value is JsonElement json ? json : null;

    private static DateOnly ExtractDate(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = GetJsonElement(data, key);
        return json.ValueKind switch
        {
            JsonValueKind.String => DateOnly.ParseExact(json.GetString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            _ => throw new InvalidOperationException($"Expected string for '{key}', got {json.ValueKind}")
        };
    }

    private static int ExtractInt(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = GetJsonElement(data, key);
        if (json.TryGetInt64(out var number))
        {
            return (int) number;
        }

        throw new InvalidOperationException($"Expected number for '{key}', got {json.ValueKind}");
    }

    private static DateOnly? ExtractNullableDate(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = TryGetJsonElement(data, key);
        if (json is null)
        {
            return null;
        }

        return json.Value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.String => DateOnly.ParseExact(json.Value.GetString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            _ => throw new InvalidOperationException($"Expected string or null for '{key}', got {json.Value.ValueKind}")
        };
    }

    private static Money ExtractMoney(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = GetJsonElement(data, key);
        if (json.TryGetInt64(out var cents))
        {
            return Money.Create(cents / 100m);
        }

        throw new InvalidOperationException($"Expected number for '{key}', got {json.ValueKind}");
    }

    private static string? ExtractString(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = TryGetJsonElement(data, key);
        if (json is null)
        {
            return null;
        }

        return json.Value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.String => json.Value.GetString(),
            _ => json.Value.ToString()
        };
    }

    private static DateIntervalType ExtractDateIntervalType(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = GetJsonElement(data, key);
        return json.ValueKind switch
        {
            JsonValueKind.String => DateIntervalType.FromName(json.GetString(), true),
            _ => throw new InvalidOperationException($"Expected DateIntervalType for '{key}', got {json.ValueKind}")
        };
    }

    private static AccountId ExtractAccountId(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = GetJsonElement(data, key);
        return json.ValueKind switch
        {
            JsonValueKind.String => AccountId.Create(Guid.Parse(json.GetString()!)),
            _ => throw new InvalidOperationException($"Expected string for '{key}', got {json.ValueKind}")
        };
    }

    private static CategoryId ExtractCategoryId(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = GetJsonElement(data, key);
        return json.ValueKind switch
        {
            JsonValueKind.String => CategoryId.Create(Guid.Parse(json.GetString()!)),
            _ => throw new InvalidOperationException($"Expected string for '{key}', got {json.ValueKind}")
        };
    }

    private static CashflowId? ExtractCashflowId(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = TryGetJsonElement(data, key);
        if (json is null)
        {
            return null;
        }

        return json.Value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.String => CashflowId.Create(Guid.Parse(json.Value.GetString()!)),
            _ => throw new InvalidOperationException($"Expected string or null for '{key}', got {json.Value.ValueKind}")
        };
    }

    private static string[] ExtractTags(IReadOnlyDictionary<string, object> data, string key)
    {
        var json = TryGetJsonElement(data, key);
        if (json is null)
        {
            return [];
        }

        return json.Value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => [],
            JsonValueKind.String => (json.Value.GetString() ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            JsonValueKind.Array => json.Value.EnumerateArray().Select(e => e.GetString() ?? string.Empty).ToArray(),
            _ => throw new InvalidOperationException($"Expected string, array, or null for '{key}', got {json.Value.ValueKind}")
        };
    }

    public record Request
    {
        [JsonPropertyName("transaction_id")] public long TransactionId { get; init; }

        [JsonPropertyName("operations")] public IReadOnlyCollection<SyncOperation> Operations { get; init; } = [];
    }

    public record SyncOperation
    {
        // The table name in the local SQLite (should match your Postgres table)
        [JsonPropertyName("type")] public string Type { get; init; } = string.Empty;

        // The operation type: "PUT", "PATCH", or "DELETE"
        [JsonPropertyName("op")] public string Op { get; init; } = string.Empty;

        // The UUID generated by the mobile app
        [JsonPropertyName("id")] public Guid Id { get; init; } = Guid.Empty;

        // The actual record data as a flexible JSON object
        [JsonPropertyName("data")] public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.TransactionId).GreaterThan(0);
            RuleForEach(command => command.Operations)
                .ChildRules(operation =>
                {
                    operation.RuleFor(op => op.Type)
                        .NotEmpty()
                        .Must(type => type is "accounts" or "categories" or "cashflows" or "store_items" or "transactions")
                        .WithMessage("Table must be one of: accounts, categories, cashflows, store_items or transactions");

                    operation.RuleFor(op => op.Op)
                        .NotEmpty()
                        .Must(op => op is "PUT" or "PATCH" or "DELETE")
                        .WithMessage("Operation must be one of: PUT, PATCH, or DELETE");

                    operation.RuleFor(op => op.Id)
                        .NotEmpty()
                        .WithMessage("Id cannot be empty");
                });
        }
    }

    [LoggerMessage(LogLevel.Information, "PowerSync completed with result: {result}")]
    static partial void LogPowersyncCompletedWithResultResult(ILogger<PowerSync> logger, Result<Nothing> result);

    [LoggerMessage(LogLevel.Information, "Starting {operations} operations for power sync for user {userId}")]
    static partial void LogStartingOperationsOperationsForPowerSyncForUserUserid(ILogger logger, int operations, UserId userId);

    [LoggerMessage(LogLevel.Error, "Error processing store item operation with id {id} for user {userId}")]
    static partial void LogErrorProcessingStoreItemOperationWithIdIdForUserUserid(ILogger logger, Guid id, UserId userId);

    [LoggerMessage(LogLevel.Error, "Error processing transaction operation with id {id} for user {userId}")]
    static partial void LogErrorProcessingTransactionOperationWithIdIdForUserUserid(ILogger logger, Guid id, UserId userId);

    [LoggerMessage(LogLevel.Error, "Error processing cashflow operation with id {id} for user {userId}")]
    static partial void LogErrorProcessingCashflowOperationWithIdIdForUserUserid(ILogger logger, Guid id, UserId userId);
}

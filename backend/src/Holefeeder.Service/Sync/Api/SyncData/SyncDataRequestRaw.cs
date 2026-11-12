using System.Globalization;
using System.Text.Json;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Sync.Core;
using Holefeeder.Service.Sync.Core.SyncData;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Sync.Api.SyncData;

internal sealed record SyncDataRequestRaw([FromBody] SyncDataBody Body)
{
    public static Result<SyncDataRequest> ToSyncDataRequest(SyncDataRequestRaw raw, User user)
    {
        var validation = ResultAggregate.Create();

        if (!long.TryParse(raw.Body.TransactionId, NumberStyles.None, CultureInfo.InvariantCulture, out var transactionId) || transactionId <= 0)
        {
            validation.AddResult(SyncDataErrors.TransactionIdInvalid);
        }

        var operations = (raw.Body.Operations ?? [])
            .Select(operation => ToSyncOperation(operation, validation))
            .ToList();

        return validation.OnSuccess<SyncDataRequest>(() => new SyncDataRequest(user, operations.OfType<SyncOperation>().ToList()));
    }

    private static SyncOperation? ToSyncOperation(SyncOperationBody body, ResultAggregate validation)
    {
        var kind = body.Op switch
        {
            "PUT" => SyncOperationKind.Put,
            "PATCH" => SyncOperationKind.Patch,
            "DELETE" => SyncOperationKind.Delete,
            _ => (SyncOperationKind?)null
        };
        if (kind is null)
        {
            validation.AddResult(SyncDataErrors.OperationInvalid);
        }

        var id = Id.Create(body.Id);
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        var fields = new SyncDataFields(
            kind == SyncOperationKind.Delete ? new Dictionary<string, JsonElement>() : body.Data ?? new Dictionary<string, JsonElement>(),
            validation);
        ISyncPatch? patch = body.Type switch
        {
            "accounts" => ToAccountPatch(fields),
            "categories" => ToCategoryPatch(fields),
            "store_items" => new SyncStoreItemPatch(fields.Text("code"), fields.Text("data")),
            "transactions" => ToTransactionPatch(fields),
            "cashflows" => ToCashflowPatch(fields),
            _ => null
        };
        if (patch is null)
        {
            validation.AddResult(SyncDataErrors.TypeInvalid);
        }

        return kind is { } validKind && id.IsSuccess && patch is not null ? new SyncOperation(validKind, id.Value, patch) : null;
    }

    private static SyncAccountPatch ToAccountPatch(SyncDataFields fields) =>
        new(
            fields.AccountType("type"),
            fields.Text("name"),
            fields.Cents("open_balance"),
            fields.Date("open_date"),
            fields.Text("description"),
            fields.Flag("favorite"),
            fields.Flag("inactive"));

    private static SyncCategoryPatch ToCategoryPatch(SyncDataFields fields) =>
        new(
            fields.CategoryType("type"),
            fields.Text("name"),
            fields.Text("color"),
            fields.Cents("budget_amount"),
            fields.Flag("favorite"),
            fields.Flag("system"),
            fields.Flag("inactive"));

    private static SyncTransactionPatch ToTransactionPatch(SyncDataFields fields) =>
        new(
            fields.Date("date"),
            fields.Cents("amount"),
            fields.Text("description"),
            fields.Reference("account_id"),
            fields.Reference("category_id"),
            fields.Reference("cashflow_id"),
            fields.Date("cashflow_date"),
            fields.Tags("tags"));

    private static SyncCashflowPatch ToCashflowPatch(SyncDataFields fields) =>
        new(
            fields.Date("effective_date"),
            fields.IntervalType("interval_type"),
            fields.Number("frequency"),
            fields.Number("recurrence"),
            fields.Cents("amount"),
            fields.Text("description"),
            fields.Reference("account_id"),
            fields.Reference("category_id"),
            fields.Flag("inactive"),
            fields.Tags("tags"));
}

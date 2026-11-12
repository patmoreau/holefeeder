using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Sync.Api.SyncData;

internal static class SyncDataErrors
{
    public const string CodeTransactionIdInvalid = "sync-transaction-id-invalid";
    public const string CodeTypeInvalid = "sync-type-invalid";
    public const string CodeOperationInvalid = "sync-operation-invalid";
    public const string CodeFieldInvalid = "sync-field-invalid";

    public static ResultError TransactionIdInvalid => new(CodeTransactionIdInvalid, "Transaction id must be a number greater than zero.");

    public static ResultError TypeInvalid =>
        new(CodeTypeInvalid, "Type must be one of accounts, categories, cashflows, store_items or transactions.");

    public static ResultError OperationInvalid => new(CodeOperationInvalid, "Operation must be one of PUT, PATCH or DELETE.");

    public static ResultError FieldInvalid(string field, string expected) =>
        new(CodeFieldInvalid, $"Field {field} must be {expected}.");
}

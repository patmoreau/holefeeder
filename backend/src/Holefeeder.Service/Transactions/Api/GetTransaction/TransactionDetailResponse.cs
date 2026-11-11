namespace Holefeeder.Service.Transactions.Api.GetTransaction;

internal sealed record TransactionDetailResponse(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string Description,
    IReadOnlyList<string> Tags,
    TransactionDetailCategoryResponse Category,
    TransactionDetailAccountResponse Account);

internal sealed record TransactionDetailCategoryResponse(Guid Id, string Name, string Type, string Color);

internal sealed record TransactionDetailAccountResponse(Guid Id, string Name);

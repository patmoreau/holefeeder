namespace Holefeeder.Service.Transactions.Api.GetNewTransactionOptions;

internal sealed record NewTransactionOptionsResponse(
    IReadOnlyList<AccountOptionResponse> Accounts,
    IReadOnlyList<CategoryOptionResponse> Categories);

internal sealed record AccountOptionResponse(Guid Id, string Name);

internal sealed record CategoryOptionResponse(Guid Id, string Name, string Color, string Type);

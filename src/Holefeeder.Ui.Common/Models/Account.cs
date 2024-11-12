namespace Holefeeder.Ui.Common.Models;

public class Account
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required AccountType Type { get; init; }
    public required decimal OpenBalance { get; init; }
    public required DateOnly OpenDate { get; init; }
    public required int TransactionCount { get; init; }
    public required decimal Balance { get; init; }
    public required DateOnly Updated { get; init; }
    public required string Description { get; init; }
    public required bool Favorite { get; init; }
    public required bool Inactive { get; init; }
}

using System;

namespace Holefeeder.Ui.Common.Models;

public class Account
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required AccountType Type { get; init; }
    public required double OpenBalance { get; init; }
    public required DateOnly OpenDate { get; init; }
    public required int TransactionCount { get; init; }
    public required double Balance { get; init; }
    public required DateOnly Updated { get; init; }
    public required string Description { get; init; }
    public required bool Favorite { get; init; }
    public required bool Inactive { get; init; }
}

namespace Holefeeder.Service.Shared.Core;

internal static class AccountBalance
{
    public static decimal Of(AccountType type, decimal openBalance, decimal gains, decimal expenses) =>
        type.Sign * openBalance + gains - expenses;

    public static BalanceCaption CaptionOf(AccountType type, decimal balance) =>
        Math.Round(balance, 2) switch
        {
            < 0 => BalanceCaption.AmountDue,
            > 0 => type.IsAsset ? BalanceCaption.AvailableBalance : BalanceCaption.CreditBalance,
            _ => type.IsAsset ? BalanceCaption.AvailableBalance : BalanceCaption.AmountDue
        };
}

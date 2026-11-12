using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Users.Core;

internal sealed record SystemCategory(Id Id, CategoryType Type, string Name)
{
    public const string TransferOutName = "Transfer Out";
    public const string TransferInName = "Transfer In";
    public const string Color = "#607D8B";
}

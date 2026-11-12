using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Core.GetImportStatus;

internal sealed record GetImportStatusRequest(User User, Id ImportId);

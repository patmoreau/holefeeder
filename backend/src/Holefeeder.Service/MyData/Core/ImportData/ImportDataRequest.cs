using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Core.ImportData;

internal sealed record ImportDataRequest(User User, bool UpdateExisting, MyDataSet Data);

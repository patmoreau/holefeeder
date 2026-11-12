using Hangfire;
using Hangfire.Common;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.MyData.Jobs.ImportData;

using static Holefeeder.ServiceTests.MyData.TestKit.MyDataForTest;

namespace Holefeeder.ServiceTests.MyData.Jobs.ImportData;

public class ImportDataPayloadTests
{
    [Fact]
    public void ToRequest_RebuildsTheRequest()
    {
        var importId = AnId();
        var request = ARequest();

        var payload = ImportDataPayload.From(importId, request);

        payload.ImportId.Should().Be(importId.Value);
        payload.ToRequest().Should().BeEquivalentTo(request);
    }

    [Fact]
    public void ToRequest_RebuildsTheRequest_AfterHangfireStoresIt()
    {
        GlobalConfiguration.Configuration.UseRecommendedSerializerSettings();
        var request = ARequest();
        var serialized = SerializationHelper.Serialize(ImportDataPayload.From(AnId(), request), SerializationOption.User);

        var payload = SerializationHelper.Deserialize<ImportDataPayload>(serialized, SerializationOption.User);

        payload.ToRequest().Should().BeEquivalentTo(request);
    }

    private static ImportDataRequest ARequest()
    {
        var account = AMyDataAccount();
        var category = AMyDataCategory();
        var payment = AMyDataTransaction(account.Id, category.Id) with { CashflowId = AnId(), CashflowDate = new DateOnly(2024, 2, 1) };
        return new ImportDataRequest(AUser(), UpdateExisting: true, new MyDataSet([account], [category],
            [AMyDataCashflow(account.Id, category.Id)], [AMyDataTransaction(account.Id, category.Id), payment]));
    }
}

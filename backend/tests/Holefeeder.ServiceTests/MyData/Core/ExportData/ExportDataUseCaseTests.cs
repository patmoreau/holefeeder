using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ExportData;
using Holefeeder.ServiceTests.MyData.TestKit;

using static Holefeeder.ServiceTests.MyData.TestKit.MyDataForTest;

namespace Holefeeder.ServiceTests.MyData.Core.ExportData;

public class ExportDataUseCaseTests
{
    [Fact]
    public async Task HandleAsync_ReturnsTheUsersData()
    {
        var user = AUser();
        var (account, category) = (AMyDataAccount(), AMyDataCategory());
        var data = new MyDataSet([account], [category], [AMyDataCashflow(account.Id, category.Id)],
            [AMyDataTransaction(account.Id, category.Id)]);
        var useCase = new ExportDataUseCase(new MyDataInMemory().With(user, data).With(AUser(), new MyDataSet([AMyDataAccount()], [], [], [])));

        var result = await useCase.HandleAsync(new ExportDataRequest(user), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyData_WhenTheUserHasNone()
    {
        var useCase = new ExportDataUseCase(new MyDataInMemory());

        var result = await useCase.HandleAsync(new ExportDataRequest(AUser()), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new MyDataSet([], [], [], []));
    }
}

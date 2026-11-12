using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.MyData.Core.ScheduleImport;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.MyData.TestKit;

using static Holefeeder.ServiceTests.MyData.TestKit.MyDataForTest;

namespace Holefeeder.ServiceTests.MyData.Core.ScheduleImport;

public class ScheduleImportUseCaseTests
{
    private readonly ImportStatusesInMemory _statuses = new();
    private readonly ImportQueueInMemory _queue = new();

    [Fact]
    public void Handle_QueuesTheImport()
    {
        var request = new ImportDataRequest(AUser(), UpdateExisting: true, new MyDataSet([AMyDataAccount()], [], [], []));

        var result = UseCase().Handle(request);

        result.Should().BeSuccessful();
        _queue.Queued.Should().Equal((result.Value, request));
    }

    [Fact]
    public void Handle_RecordsTheImportAsInitializing()
    {
        var user = AUser();

        var result = UseCase().Handle(new ImportDataRequest(user, UpdateExisting: false, new MyDataSet([AMyDataAccount()], [], [], [])));

        _statuses.Find(result.Value, user.Id).Should().BeSuccessful()
            .And.WithValue(ImportStatus.Initializing(result.Value, user.Id));
    }

    [Fact]
    public void Handle_FailsWithNotRegistered_WhenTheUserIsNotRegistered()
    {
        var result = UseCase().Handle(new ImportDataRequest(User.NoUser, UpdateExisting: false,
            new MyDataSet([AMyDataAccount()], [], [], [])));

        result.Should().BeFailure().And.WithError(UserErrors.NotRegistered);
        _queue.Queued.Should().BeEmpty();
    }

    private ScheduleImportUseCase UseCase() => new(_statuses, _queue);
}

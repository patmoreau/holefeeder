using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.MyData.TestKit;

using static Holefeeder.ServiceTests.MyData.TestKit.MyDataForTest;

namespace Holefeeder.ServiceTests.MyData.Core.ImportData;

public class ImportDataUseCaseTests
{
    private readonly User _user = AUser();
    private readonly Id _importId = AnId();
    private readonly MyDataInMemory _store = new();
    private readonly ImportStatusesInMemory _statuses = new();

    [Fact]
    public async Task HandleAsync_ImportsEveryTable()
    {
        var data = AFullSet();

        var result = await Import(data, updateExisting: false);

        result.Should().BeSuccessful();
        _store.DataOf(_user).Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task HandleAsync_CompletesTheStatusWithTheProgressOfEachTable()
    {
        var account = AMyDataAccount();
        var category = AMyDataCategory();
        var data = new MyDataSet([account, AMyDataAccount()], [category], [], [AMyDataTransaction(account.Id, category.Id)]);

        await Import(data, updateExisting: false);

        TheStatus().Should().Be(new ImportStatus(_importId, _user.Id, ImportState.Completed, new ImportProgress(2, 2),
            new ImportProgress(1, 1), ImportProgress.None, new ImportProgress(1, 1), string.Empty));
    }

    [Fact]
    public async Task HandleAsync_ReportsProgressWhileImporting()
    {
        await Import(new MyDataSet([AMyDataAccount(), AMyDataAccount()], [], [], []), updateExisting: false);

        _statuses.History.Select(status => (status.State, status.Accounts)).Should().Equal(
            (ImportState.InProgress, ImportProgress.None),
            (ImportState.InProgress, new ImportProgress(0, 2)),
            (ImportState.InProgress, new ImportProgress(1, 2)),
            (ImportState.InProgress, new ImportProgress(2, 2)),
            (ImportState.Completed, new ImportProgress(2, 2)));
    }

    [Fact]
    public async Task HandleAsync_KeepsExistingRecords_WhenNotUpdatingExisting()
    {
        var existing = AMyDataAccount();
        _store.With(_user, new MyDataSet([existing], [], [], []));

        var result = await Import(new MyDataSet([existing with { Name = "Renamed" }], [], [], []), updateExisting: false);

        result.Should().BeSuccessful();
        _store.DataOf(_user).Accounts.Should().Equal(existing);
        TheStatus().Accounts.Should().Be(new ImportProgress(1, 1));
    }

    [Fact]
    public async Task HandleAsync_ReplacesExistingRecords_WhenUpdatingExisting()
    {
        var existing = AMyDataAccount();
        _store.With(_user, new MyDataSet([existing], [], [], []));
        var changed = existing with { Name = "Renamed" };

        var result = await Import(new MyDataSet([changed], [], [], []), updateExisting: true);

        result.Should().BeSuccessful();
        _store.DataOf(_user).Accounts.Should().Equal(changed);
    }

    [Fact]
    public async Task HandleAsync_FailsAndImportsNothing_WhenAnIdBelongsToAnotherUser()
    {
        var theirs = AMyDataCategory();
        var otherUser = AUser();
        _store.With(otherUser, new MyDataSet([], [theirs], [], []));

        var result = await Import(new MyDataSet([AMyDataAccount()], [theirs with { Name = "Mine" }], [], []), updateExisting: true);

        result.Should().BeFailure().And.WithError(MyDataImportErrors.IdTaken(MyDataTable.Categories));
        _store.DataOf(_user).Should().BeEquivalentTo(new MyDataSet([], [], [], []));
        _store.DataOf(otherUser).Categories.Should().Equal(theirs);
    }

    [Fact]
    public async Task HandleAsync_EndsTheStatusInError_WhenTheImportFails()
    {
        var theirs = AMyDataAccount();
        _store.With(AUser(), new MyDataSet([theirs], [], [], []));

        await Import(new MyDataSet([theirs], [], [], []), updateExisting: true);

        var error = MyDataImportErrors.IdTaken(MyDataTable.Accounts);
        TheStatus().Should().Be(new ImportStatus(_importId, _user.Id, ImportState.Error, new ImportProgress(0, 1), ImportProgress.None,
            ImportProgress.None, ImportProgress.None, $"{error.Code}: {error.Description}"));
    }

    private Task<Result<Nothing>> Import(MyDataSet data, bool updateExisting) =>
        new ImportDataUseCase(_store, _statuses).HandleAsync(_importId, new ImportDataRequest(_user, updateExisting, data),
            CancellationToken.None);

    private ImportStatus TheStatus() => _statuses.Find(_importId, _user.Id).Value;

    private static MyDataSet AFullSet()
    {
        var account = AMyDataAccount();
        var category = AMyDataCategory();
        return new MyDataSet([account], [category], [AMyDataCashflow(account.Id, category.Id)],
            [AMyDataTransaction(account.Id, category.Id)]);
    }
}

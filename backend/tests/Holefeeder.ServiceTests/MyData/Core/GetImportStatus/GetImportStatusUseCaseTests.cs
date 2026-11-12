using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.GetImportStatus;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.MyData.TestKit;

namespace Holefeeder.ServiceTests.MyData.Core.GetImportStatus;

public class GetImportStatusUseCaseTests
{
    private readonly ImportStatusesInMemory _statuses = new();

    [Fact]
    public void Handle_ReturnsTheStatusOfTheUsersImport()
    {
        var user = AUser();
        var status = ImportStatus.Initializing(AnId(), user.Id);
        _statuses.Save(status);

        var result = new GetImportStatusUseCase(_statuses).Handle(new GetImportStatusRequest(user, status.ImportId));

        result.Should().BeSuccessful().And.WithValue(status);
    }

    [Fact]
    public void Handle_FailsWithNotFound_WhenTheImportBelongsToAnotherUser()
    {
        var status = ImportStatus.Initializing(AnId(), AUser().Id);
        _statuses.Save(status);

        var result = new GetImportStatusUseCase(_statuses).Handle(new GetImportStatusRequest(AUser(), status.ImportId));

        result.Should().BeFailure().And.WithError(ImportStatusErrors.NotFound);
    }

    [Fact]
    public void Handle_FailsWithNotFound_WhenTheImportIsUnknown()
    {
        var result = new GetImportStatusUseCase(_statuses).Handle(new GetImportStatusRequest(AUser(), AnId()));

        result.Should().BeFailure().And.WithError(ImportStatusErrors.NotFound);
    }

    [Fact]
    public void NotFound_IsANotFoundError() =>
        ImportStatusErrors.NotFound.Should().BeOfType<NotFoundError>();
}

using Holefeeder.Application.Features.MyData.Models;

namespace Holefeeder.Application.Features.MyData;

public interface IMyDataQueriesRepository
{
    Task<IEnumerable<MyDataAccountDto>> ExportAccountsAsync(Guid userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MyDataCategoryDto>> ExportCategoriesAsync(Guid userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MyDataCashflowDto>> ExportCashflowsAsync(Guid userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MyDataTransactionDto>> ExportTransactionsAsync(Guid userId,
        CancellationToken cancellationToken = default);
}

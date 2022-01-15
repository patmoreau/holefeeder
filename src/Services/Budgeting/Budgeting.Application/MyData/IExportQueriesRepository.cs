using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData;

public interface IExportQueriesRepository
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

using System.Collections.Immutable;

using Holefeeder.Application.Features.MyData.Models;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;

public record ExportDataDto(
    ImmutableArray<MyDataAccountDto> Accounts,
    ImmutableArray<MyDataCategoryDto> Categories,
    ImmutableArray<MyDataCashflowDto> Cashflows,
    ImmutableArray<MyDataTransactionDto> Transactions);

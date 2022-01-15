using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;

public record ExportDataDto(
    ImmutableArray<MyDataAccountDto> Accounts,
    ImmutableArray<MyDataCategoryDto> Categories,
    ImmutableArray<MyDataCashflowDto> Cashflows,
    ImmutableArray<MyDataTransactionDto> Transactions);

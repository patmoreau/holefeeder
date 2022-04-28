using System.Collections.Immutable;

namespace Holefeeder.Application.Features.MyData.Models;

public record ExportDataDto(
    ImmutableArray<MyDataAccountDto> Accounts,
    ImmutableArray<MyDataCategoryDto> Categories,
    ImmutableArray<MyDataCashflowDto> Cashflows,
    ImmutableArray<MyDataTransactionDto> Transactions);

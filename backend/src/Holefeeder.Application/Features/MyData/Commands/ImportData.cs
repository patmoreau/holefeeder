using System.ComponentModel.DataAnnotations;

using Holefeeder.Application.Features.MyData.Models;

namespace Holefeeder.Application.Features.MyData.Commands;

public static class ImportData
{
    internal record Request
    {
        [Required] public bool UpdateExisting { get; init; }

        [Required] public Dto Data { get; init; } = null!;

        public record Dto
        {
            public MyDataAccountDto[] Accounts { get; init; } = [];
            public MyDataCategoryDto[] Categories { get; init; } = [];
            public MyDataCashflowDto[] Cashflows { get; init; } = [];
            public MyDataTransactionDto[] Transactions { get; init; } = [];
        }
    }
}

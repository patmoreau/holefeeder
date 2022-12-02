using System.ComponentModel.DataAnnotations;

using Holefeeder.Application.Features.MyData.Models;

namespace Holefeeder.Application.Features.MyData.Commands.ImportData;

public record Request : IRequest
{
    [Required] public bool UpdateExisting { get; init; }

    [Required] public Dto Data { get; init; } = null!;

    public record Dto
    {
        public MyDataAccountDto[] Accounts { get; init; } = Array.Empty<MyDataAccountDto>();
        public MyDataCategoryDto[] Categories { get; init; } = Array.Empty<MyDataCategoryDto>();
        public MyDataCashflowDto[] Cashflows { get; init; } = Array.Empty<MyDataCashflowDto>();
        public MyDataTransactionDto[] Transactions { get; init; } = Array.Empty<MyDataTransactionDto>();
    }
}

public record InternalRequest : Request
{
    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }
}

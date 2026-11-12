using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions;

internal static class CashflowMapper
{
    public static MyDataCashflowDto MapToMyDataCashflowDto(Cashflow entity)
    {
        var dto = new MyDataCashflowDto
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Description = entity.Description,
            EffectiveDate = entity.EffectiveDate,
            Frequency = entity.Frequency,
            IntervalType = entity.IntervalType,
            Recurrence = entity.Recurrence,
            Tags = entity.Tags.ToArray(),
            AccountId = entity.AccountId,
            CategoryId = entity.CategoryId,
            Inactive = entity.Inactive
        };

        return dto;
    }
}

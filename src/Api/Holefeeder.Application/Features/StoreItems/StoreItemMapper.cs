using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems;

internal static class StoreItemMapper
{
    public static StoreItemViewModel? MapToDtoOrNull(StoreItem? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public static StoreItemViewModel MapToDto(StoreItem entity)
    {
        var dto = new StoreItemViewModel(entity.Id, entity.Code, entity.Data);

        return dto;
    }

    public static IEnumerable<StoreItemViewModel> MapToDto(IEnumerable<StoreItem> entities)
    {
        return entities.Select(MapToDto);
    }
}

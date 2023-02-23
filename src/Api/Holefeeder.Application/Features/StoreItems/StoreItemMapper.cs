using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems;

internal static class StoreItemMapper
{
    public static StoreItemViewModel? MapToDtoOrNull(StoreItem? entity) => entity is null ? null : MapToDto(entity);

    public static StoreItemViewModel MapToDto(StoreItem entity)
    {
        StoreItemViewModel dto = new StoreItemViewModel(entity.Id, entity.Code, entity.Data);

        return dto;
    }

    public static IEnumerable<StoreItemViewModel> MapToDto(IEnumerable<StoreItem> entities) =>
        entities.Select(MapToDto);
}

using Holefeeder.Application.Domain.StoreItem;
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

    public static StoreItem? MapToModelOrNull(StoreItem? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var model = new StoreItem(entity.Id, entity.Code, entity.UserId) {Data = entity.Data};

        return model;
    }
    //
    // public static StoreItem MapToEntity(StoreItem model)
    // {
    //     return new StoreItem {Id = model.Id, Code = model.Code, Data = model.Data, UserId = model.UserId};
    // }
}

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Infrastructure.Mapping;

internal static class StoreItemMapper
{
    public static StoreItemViewModel? MapToDtoOrNull(StoreItemEntity? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public static StoreItemViewModel MapToDto(StoreItemEntity entity)
    {
        var dto = new StoreItemViewModel(entity.Id, entity.Code, entity.Data);

        return dto;
    }

    public static IEnumerable<StoreItemViewModel> MapToDto(IEnumerable<StoreItemEntity> entities)
    {
        return entities.Select(MapToDto);
    }

    public static StoreItem? MapToModelOrNull(StoreItemEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var model = new StoreItem(entity.Id, entity.Code, entity.UserId) {Data = entity.Data};

        return model;
    }

    public static StoreItemEntity MapToEntity(StoreItem model)
    {
        return new StoreItemEntity {Id = model.Id, Code = model.Code, Data = model.Data, UserId = model.UserId};
    }
}

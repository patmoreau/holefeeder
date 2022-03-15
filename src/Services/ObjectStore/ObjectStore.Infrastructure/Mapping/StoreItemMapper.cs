using System.Collections.Generic;
using System.Linq;

using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Mapping;

public class StoreItemMapper
{
    public StoreItemViewModel? MapToDtoOrNull(StoreItemEntity? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public StoreItemViewModel MapToDto(StoreItemEntity entity)
    {
        var dto = new StoreItemViewModel(entity.Id, entity.Code, entity.Data);

        return dto;
    }

    public IEnumerable<StoreItemViewModel> MapToDto(IEnumerable<StoreItemEntity> entities)
    {
        return entities.Select(MapToDto);
    }

    public StoreItem? MapToModelOrNull(StoreItemEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var model = new StoreItem {Id = entity.Id, Code = entity.Code, Data = entity.Data, UserId = entity.UserId};

        return model;
    }

    public StoreItemEntity MapToEntity(StoreItem model)
    {
        return new StoreItemEntity {Id = model.Id, Code = model.Code, Data = model.Data, UserId = model.UserId};
    }
}

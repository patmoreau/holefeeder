using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Infrastructure.Mapping;

internal class StoreItemMapper
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

        var model = new StoreItem(entity.Id, entity.Code, entity.UserId) {Data = entity.Data};

        return model;
    }

    public StoreItemEntity MapToEntity(StoreItem model)
    {
        return new StoreItemEntity {Id = model.Id, Code = model.Code, Data = model.Data, UserId = model.UserId};
    }
}

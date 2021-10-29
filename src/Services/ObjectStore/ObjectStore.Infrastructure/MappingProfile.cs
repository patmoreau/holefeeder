using AutoMapper;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreItemEntity, StoreItem>().ReverseMap();
            CreateMap<StoreItemEntity, StoreItemViewModel>();
        }
    }
}

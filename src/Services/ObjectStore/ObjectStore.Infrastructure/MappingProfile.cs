using AutoMapper;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreItemSchema, StoreItemViewModel>().ReverseMap();
            CreateMap<StoreItemSchema, StoreItem>().ReverseMap();
        }
    }
}

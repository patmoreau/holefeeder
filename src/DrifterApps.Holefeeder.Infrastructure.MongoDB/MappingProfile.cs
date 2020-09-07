using System;
using AutoMapper;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountSchema, AccountViewModel>()
                .ForSourceMember(x => x.CatchAll, opt => opt.DoNotValidate())
                .ForCtorParam("transactionCount", opt => opt.MapFrom(_ => 0))
                .ForCtorParam("balance", opt => opt.MapFrom(_ => 0))
                .ForCtorParam("updated", opt => opt.MapFrom(_ => DateTime.Today));
        }
    }
}

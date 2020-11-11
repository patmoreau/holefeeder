using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class DataAccessProfile : Profile
    {
        public DataAccessProfile()
        {
            CreateMap<AccountSchema, AccountEntity>()
                .ForSourceMember(x => x.CatchAll, opt => opt.DoNotValidate())
                .ReverseMap()
                .ForMember(x => x.OpenDate, opt => opt.MapFrom(y => y.OpenDate.Date));
            CreateMap<CashflowSchema, CashflowEntity>()
                .ForSourceMember(x => x.CatchAll, opt => opt.DoNotValidate())
                .ReverseMap()
                .ForMember(x => x.EffectiveDate, opt => opt.MapFrom(y => y.EffectiveDate.Date));
            CreateMap<CategorySchema, CategoryEntity>()
                .ForSourceMember(x => x.CatchAll, opt => opt.DoNotValidate())
                .ReverseMap();
            CreateMap<ObjectDataSchema, ObjectDataEntity>()
                .ForSourceMember(x => x.CatchAll, opt => opt.DoNotValidate())
                .ReverseMap();
            CreateMap<TransactionSchema, TransactionEntity>()
                .ForSourceMember(x => x.CatchAll, opt => opt.DoNotValidate())
                .ReverseMap()
                .ForMember(x => x.Date, opt => opt.MapFrom(y => y.Date.Date));
        }
    }
}

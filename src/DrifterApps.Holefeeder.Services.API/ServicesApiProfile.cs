using AutoMapper;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Services.DTO;

namespace DrifterApps.Holefeeder.Services.API
{
    public class ServicesApiProfile : Profile
    {
        public ServicesApiProfile()
        {
            CreateMap<AccountEntity, AccountDTO>().ReverseMap();
            CreateMap<CashflowEntity, CashflowDTO>();
            CreateMap<CashflowInfoEntity, CashflowInfoDTO>();
            CreateMap<CategoryEntity, CategoryInfoEntity>();
            CreateMap<CategoryEntity, CategoryDTO>().ReverseMap();
            CreateMap<CategoryInfoEntity, CategoryInfoDTO>();
            CreateMap<ObjectDataEntity, ObjectDataDTO>().ReverseMap();
            CreateMap<UserEntity, UserDTO>().ReverseMap();
            CreateMap<UpcomingEntity, UpcomingDTO>().ReverseMap();
            CreateMap<TransactionEntity, TransactionDTO>().ReverseMap();
            CreateMap<TransactionDetailEntity, TransactionDetailDTO>();
            CreateMap<AccountDetailEntity, AccountDetailDTO>();
            CreateMap<AccountInfoEntity, AccountInfoDTO>();
            CreateMap(typeof(StatisticsEntity<>), typeof(StatisticsDTO<>));
            CreateMap(typeof(SeriesEntity), typeof(SeriesDTO));
        }
    }
}
